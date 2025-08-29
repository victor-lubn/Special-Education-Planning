using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.Model;
using Version = SpecialEducationPlanning
.Domain.Entity.Version;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IVersionRepository : IBaseRepository<Version>
    {
        Task<VersionModel> CreateVersionAsync(VersionModel value);
        Task<RepositoryResponse<VersionModel>> UpdateVersionAsync(Version value);
        Task<IEnumerable<VersionModel>> GetVersionsByPlanId(int planId);
        Task<RepositoryResponse<string>> GetRomPathById(int id);
        Task<RepositoryResponse<Version>> GetRomAndPreviewInfo(int id);
        Task<RepositoryResponse<VersionModel>> SetVersionRom(int id, string path, string fileName, string versionNotes);
        Task<RepositoryResponse<VersionModel>> SetVersionPreview(int id, string path, string fileName);
        Task<RepositoryResponse<int>> GetAvailableCatalogsAsync(int id);
        Task<RepositoryResponse<VersionModel>> NewVersionAsync(int planId);
        Task<RepositoryResponse<VersionModel>> SaveVersion(int planId, int versionId, VersionInfoModel versionInfoModel);
        Task<RepositoryResponse<VersionModel>> UploadRomFile(Stream streamFileModel, VersionModel versionModel);
        Task<RepositoryResponse<VersionModel>> UploadPreviewFile(Stream streamFileModel, int versionId);
        Task<RepositoryResponse<VersionModel>> UploadRomFile(Stream romStream, string fileName, VersionModel versionModel);
        Task<RepositoryResponse<VersionModel>> UploadPreviewFile(Stream previewStream, string fileName, int versionId);
        Task<RepositoryResponse<VersionModel>> ModifyVersionNotes(int versionId, string versionNotes);
        Task<RepositoryResponse<VersionModel>> ModifyVersionNotesAndQuote(int versionId, string versionNotes, string quoteOrderNumber);
        Task CallIndexerAsync(int take, int skip, DateTime? updateDate, int? indexerWindowInDays);
        Task<Version> GetVersionWithPlanProjectAiep(int versionId);
        Task<Version> GetVersionById(int versionId);
    }
}
