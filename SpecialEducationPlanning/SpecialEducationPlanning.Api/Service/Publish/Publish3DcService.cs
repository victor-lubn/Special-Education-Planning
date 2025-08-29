using Koa.Domain.Specification;
using Koa.Persistence.EntityRepository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpecialEducationPlanning
.Api.Model.PublishServiceModel;
using SpecialEducationPlanning
.Api.Service.ServiceBus;
using SpecialEducationPlanning
.Business.Model.PublishModel;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Service.Publish
{
    public class Publish3DcService : IPublish3DcService
    {
        private readonly IPlanRepository _planRepository;
        private readonly IServiceBusService _serviceBusService;
        private readonly IEntityRepository _entityRepository;
        private readonly ILogger<Publish3DcService> _logger;
        private readonly IOptions<Publish3DcServiceConfiguration> _publish3DcServiceConfiguration;

        public Publish3DcService(
            IPlanRepository planRepository,
            IServiceBusService serviceBusService,
            ILogger<Publish3DcService> logger,
            IOptions<Publish3DcServiceConfiguration> publish3DcServiceConfiguration,
            IEntityRepository entityRepository)
        {
            _planRepository = planRepository;
            _serviceBusService = serviceBusService;
            _logger = logger;
            _publish3DcServiceConfiguration = publish3DcServiceConfiguration;
            _entityRepository = entityRepository;
        }

        public async Task<RepositoryResponse<string>> PublishVersionAsync(PublishVersionModel request)
        {
            _logger.LogDebug($"{nameof(Publish3DcService)} called {nameof(PublishVersionAsync)}");

            var spec = new Specification<Plan>(p => p.Id == request.Version.PlanId);
            var plan = await _entityRepository.Where(spec)
                .Include(p => p.Project)
                .ThenInclude(project => project.Aiep)
                .FirstOrDefaultAsync();
            
            var publish = new PublishRequestModel
            {
                RenderingTransactionId = Guid.NewGuid(),
                PlanCode = plan?.PlanCode,
                VersionCode = request.VersionCode,
                PlanName = plan?.PlanName,
                Range = request.Version.Range,
                Educationer = request.EducationerEmail,
                AiepCode = request.Version.AiepCode,
                InputStoragePath = request.Version.RomPath,
                RequestedByUser = request.UserEmail,
                PlanQuality = nameof(PlanPublicationType.StillsHQ),
                Country = request.Country,
                CadPlatform = "_3dc",
                RequestTimestamp = DateTime.UtcNow,
            };

            if (request.Destination == DestinationEnum.MY_KITCHEN)
            {
                publish.Destination = nameof(PublishDestinationType.MyKitchen);
                publish.DestinationDetails = new MyKitchenDestinationDetails
                {
                    FromEmail = request.EducationerEmail,
                    ToEmail = request.ReceipientEmail1,
                    CcEmails = [request.ReceipientEmail2],
                    AiepEmail = plan?.Project?.Aiep?.Email,
                    //ProductDataStoragePath = 
                    Comments = request.Comments
                };
            }
            else if (request.Destination == DestinationEnum.CONTRACT_HUB)
            {
                publish.Destination = nameof(PublishDestinationType.Crm);
                publish.DestinationDetails = new CrmDestinationDetails
                {
                    CrmProjectCode = request.CrmProjectCode,
                    CrmHousingSpecificationCode = request.CrmHousingSpecificationCode,
                    CrmHousingType = request.CrmHousingType
                };
            }

            await _serviceBusService.SendMessageAsync(publish, _publish3DcServiceConfiguration.Value.PublishQueueName);

            return new RepositoryResponse<string>();
        }
    }
}


