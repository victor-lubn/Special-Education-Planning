using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Model.PublishServiceModel;
using SpecialEducationPlanning
.Api.Service.FeatureManagement;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Service.Publish
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPublishService 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="publishPlanPostModel"></param>
        /// <param name="stream"></param>
        Task<RepositoryResponse<string>> PublishPlanFileAsync(PublishVersionModel publishPlan, Stream stream);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        Task<Stream> CreateLegacyZipPlanFromStream(Stream stream);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<RepositoryResponse<string>> PublishVersionAsync(PublishVersionModel request);

        Task<RepositoryResponse<string>> HealthCheck();

        Task<RepositoryResponse<PublishJobModel>> GetPublishJobByJobIdAsync(Guid jobId);

        Task<RepositoryResponse<IEnumerable<PublishJobModel>>> GetPublishJobsByVersionCodesAsync(IEnumerable<string> versionCodes);
    }
}