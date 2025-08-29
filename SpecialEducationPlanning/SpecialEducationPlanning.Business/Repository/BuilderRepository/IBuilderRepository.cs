using Koa.Domain.Search.Page;
using Koa.Domain.Search.Sort;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IBuilderRepository : IBaseRepository<Builder>
    {
        Task<RepositoryResponse<BuilderModel>> CreateAsync(BuilderModel builderModel, int EducationerId, int AiepId);
        Task<RepositoryResponse<BuilderModel>> EndUserRefreshAsync(int endUserId);
        Task<RepositoryResponse<IPagedQueryResult<BuilderModel>>> GetBuildersFiltered(IPageDescriptor model, int? currentAiepId);
        Task<RepositoryResponse<IEnumerable<BuilderModel>>> GetAiepBuilders(int customerId);
        Task<RepositoryResponse<IEnumerable<BuilderModel>>> GetProjectBuilders(int builderId);
        Task<RepositoryResponse<BuilderModel>> IsValidBuilder(BuilderModel builderModel);
        Task<RepositoryResponse<Tuple<IEnumerable<BuilderModel>, int>>> GetBuildersOmniSearch(string textToSearch, int take, int? currentAiepId);
        Task<RepositoryResponseGeneric> BuilderCleanManagment(int idBuilder);
        Task<RepositoryResponse<BuilderModel>> GetExistingBuilderAsync(BuilderModel builderModel);
        Task<RepositoryResponse<BuilderModel>> ValidateEducationerAiep(int EducationerId, int AiepId);
        /// <summary>
        /// Get the builders where the postcode matches with the postcode of the given builder
        /// </summary>
        /// <param name="builderModel"></param>
        /// <returns></returns>
        Task<RepositoryResponse<IEnumerable<BuilderModel>>> GetPosibleBuildersMatch(BuilderModel builderModel);
        /// <summary>
        /// Get the builders from tdp db that matches exactly or partialy with the given one
        /// </summary>
        /// <param name="builderModel"></param>
        /// <returns></returns>
        Task<RepositoryResponse<ValidationBuilderModel>> GetPosibleTdpMatchingBuilders(BuilderModel builderModel);

        /// <summary>
        /// Get the builders from tdp db that matches exactly with the account number
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        Task<RepositoryResponse<ValidationBuilderModel>> GetPossibleTdpMatchingBuilderByAccountNumberAsync(string accountNumber);
        /// <summary>
        /// Method to update builders with the info coming from SAP
        /// </summary>
        /// <param name="builderModels"></param>
        /// <returns></returns>
        Task<RepositoryResponse<ICollection<BuilderModel>>> UpdateBuildersFromSapAsync(IEnumerable<BuilderSapModel> builderModels);
        Task<RepositoryResponse<BuilderModel>> GetExistingBuilderOrEmptyAsync(BuilderModel builderModel);
        Task<RepositoryResponse<Builder>> GetAssignedPlansAsync(int builderId);
        RepositoryResponse<ValidationBuilderModel> MergeBuilderTdpAndSapSearch(ValidationBuilderModel tdpValidationBuilderModel, ValidationBuilderModel sapValidationBuilderModel);
        Task<RepositoryResponseGeneric> DeleteAccountNumberAsync(int idBuilder, string accountNumber);
        Task<RepositoryResponse<BuilderModel>> UpdateBuilderFromSAPByAccountNumberAsync(BuilderModel SAPBuilder, BuilderModel builderTDP);
        Task<RepositoryResponse<BuilderModel>> ModifyBuilderNotes(int builderId, string notes);
        Task<RepositoryResponse<IEnumerable<BuilderModel>>> GetBuildersByIdsAsync(IEnumerable<int> ids, int skip, int take, int AiepId, SortDescriptor azureSort);
        Task CallIndexerAsync(int take, int skip, DateTime? updatedDate, int? indexerWindowInDays);

        Task<Builder> GetExistingDbBuilderAsync(BuilderModel builderModel);

        Task<Builder> AddBuilder(Builder entity);
    }
}

