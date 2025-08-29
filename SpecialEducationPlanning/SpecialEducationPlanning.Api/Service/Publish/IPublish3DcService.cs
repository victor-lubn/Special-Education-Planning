using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Model.PublishServiceModel;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Service.Publish
{
    public interface IPublish3DcService
    {
        Task<RepositoryResponse<string>> PublishVersionAsync(PublishVersionModel request);
    }
}
