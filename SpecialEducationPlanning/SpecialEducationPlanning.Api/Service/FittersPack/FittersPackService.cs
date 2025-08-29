using Hangfire;
using Koa.Domain.Specification;
using Koa.Persistence.EntityRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.FittersPack;
using SpecialEducationPlanning
.Api.Model.FittersPackModel;
using SpecialEducationPlanning
.Api.Service.ThreeDc;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using Version = SpecialEducationPlanning
.Domain.Entity.Version;

namespace SpecialEducationPlanning
.Api.Service.FittersPack
{
    public class FittersPackService : HangfireRetryHandlerBase<FittersPackContext>, IFittersPackService
    {
        private readonly IVersionRepository _versionRepository;
        private readonly IThreeDcApiService _threeDcApiService;
        private readonly IEntityRepository _entityRepository;
        private readonly ILogger<FittersPackService> _logger;
        private readonly IOptions<FittersPackConfiguration> _fittersPackConfiguration;
        private readonly IList<string> _overdueRetryStatuses;

        public FittersPackService(
            IVersionRepository versionRepository,
            ILogger<FittersPackService> logger,
            IThreeDcApiService threeDcApiService,
            IEntityRepository entityRepository,
            IOptions<FittersPackConfiguration> fittersPackConfiguration,
            IBackgroundJobClient backgroundJobClient,
            IConfiguration configuration)
            : base(backgroundJobClient, logger, fittersPackConfiguration.Value.HangfireRetry)
        {
            _versionRepository = versionRepository;
            _logger = logger;
            _threeDcApiService = threeDcApiService;
            _entityRepository = entityRepository;
            _fittersPackConfiguration = fittersPackConfiguration;
            _overdueRetryStatuses = fittersPackConfiguration.Value.OverdueRetryStatuses ?? new List<string> { nameof(FittersPackStatusEnum.Queued) };
        }

        public async Task GenerateFitterPackAsync(int versionId, int EducationTool3DCVersionId, FitterPackProcessType processType = FitterPackProcessType.Generate)
        {
            if (!_fittersPackConfiguration.Value.Enabled)
            {
                return;
            }

            _logger.LogDebug($"{nameof(FittersPackService)} called {nameof(GenerateFitterPackAsync)}. VersionId: {versionId}, EducationTool3DCVersionId: {EducationTool3DCVersionId}");
            var context = new FittersPackContext(versionId, EducationTool3DCVersionId, processType);

            await ExecuteWithRetryAsync(
                context,
                0
            );
        }

        public override async Task OnFailAsync(FittersPackContext context)
        {
            _logger.LogWarning($"Error occurred while executing attempt:3 of generating Fitter Pack. VersionId: {context.VersionId}, EducationTool3DCVersionId: {context.EducationTool3DCVersionId}");

            var spec = new Specification<Version>(p =>
                p.Id == context.VersionId && p.EducationTool3DCVersionId == context.EducationTool3DCVersionId);
            var version = await _entityRepository
                .Where(spec)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync();

            if (version == null)
            {
                _logger.LogInformation($"{nameof(OnFailAsync)} skipped as version was not found. VersionId: {context.VersionId}, EducationTool3DCVersionId: {context.EducationTool3DCVersionId}");
                return;
            }

            var statusName = context.ProcessType == FitterPackProcessType.Overdue
                ? nameof(FittersPackStatusEnum.OverDueFailed)
                : nameof(FittersPackStatusEnum.RetryFailed);

            version.FittersPackStatusId = await GetFittersPackStatusIdAsync(statusName);

            await _versionRepository.UpdateVersionAsync(version);
        }

        public override async Task ExecuteAsync(FittersPackContext context)
        {
            var spec = new Specification<Version>(p =>
                p.Id == context.VersionId && p.EducationTool3DCVersionId == context.EducationTool3DCVersionId);
            var version = await _entityRepository
                .Where(spec)
                .Include(v => v.FittersPackStatus)
                .Include(v => v.Plan)
                    .ThenInclude(p => p.Educationer)
                    .ThenInclude(d => d.Aiep)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync();

            if (version == null)
            {
                _logger.LogInformation(
                    $"Skipped Generation FittersPack as version was not found. VersionId: {context.VersionId}, EducationTool3DCVersionId: {context.EducationTool3DCVersionId}");
                return;
            }

            if (context.ProcessType == FitterPackProcessType.Generate && version.FittersPackStatusId.HasValue)
            {
                _logger.LogInformation($"Skipped Generation FittersPack as version already has FittersPackStatus. VersionId: {context.VersionId}, EducationTool3DCVersionId: {context.EducationTool3DCVersionId}, FittersPackStatus: {version.FittersPackStatus?.Name}.");
                return;
            }

            if (context.ProcessType == FitterPackProcessType.Overdue && !_overdueRetryStatuses.Contains(version.FittersPackStatus?.Name))
            {
                _logger.LogInformation($"Skipped Overdue Generation FittersPack as version has not Overdue FittersPackStatus. VersionId: {context.VersionId}, EducationTool3DCVersionId: {context.EducationTool3DCVersionId}, FittersPackStatus: {version.FittersPackStatus?.Name}.");
                return;
            }

            var model = PopulateFittersPackRequestModel(version);

            var response = await _threeDcApiService.GenerateFitterPack(model);
            if (response.HasError() || response.Content.IsNull())
            {
                throw new GenerateFitterPackException(string.Join(", ", response.ErrorList));
            }
            var fittersPackInfo = response.Content;
            version.FittersPack3DCEstimatedTime = fittersPackInfo.EstimatedCompletionTime;
            version.FittersPack3DCJobId = fittersPackInfo.JobId;
            version.FittersPack3DCRequestTime = DateTime.UtcNow;

            var statusName = context.ProcessType == FitterPackProcessType.Overdue
    ? nameof(FittersPackStatusEnum.OverDueQueued)
    : nameof(FittersPackStatusEnum.Queued);

            version.FittersPackStatusId = await GetFittersPackStatusIdAsync(statusName);

            await _versionRepository.UpdateVersionAsync(version);

        }

        private GenerateFittersPackRequestModel PopulateFittersPackRequestModel(Version version)
        {
            var model = new GenerateFittersPackRequestModel
            {
                PlanId = version.EducationTool3DCPlanId,
                VersionId = version.EducationTool3DCVersionId.Value,
                EducationerDetails = new GenerateFitterPackEducationerDetails
                {
                    EducationerName = $"{version.Plan.Educationer.FirstName} {version.Plan.Educationer.Surname}"
                },
                AiepDetails = new GenerateFitterPackAiepDetails
                {
                    Address1 = version.Plan.Educationer.Aiep.Address1,
                    Address2 = version.Plan.Educationer.Aiep.Address2,
                    Address3 = version.Plan.Educationer.Aiep.Address3,
                    Address4 = version.Plan.Educationer.Aiep.Address4,
                    Address5 = version.Plan.Educationer.Aiep.Address5,
                    Address6 = version.Plan.Educationer.Aiep.Address6,
                    Email = version.Plan.Educationer.Aiep.Email,
                    Name = version.Plan.Educationer.Aiep.Name,
                    PhoneNumber = version.Plan.Educationer.Aiep.PhoneNumber,
                    Postcode = version.Plan.Educationer.Aiep.Postcode
                }
            };
            return model;
        }

        private async Task<int?> GetFittersPackStatusIdAsync(string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return null;
            }

            var spec = new Specification<FittersPackStatus>(p => p.Name == status);
            var fittersPackStatus = await _entityRepository.Where(spec)
                .FirstOrDefaultAsync();

            return fittersPackStatus?.Id;
        }
    }
}


