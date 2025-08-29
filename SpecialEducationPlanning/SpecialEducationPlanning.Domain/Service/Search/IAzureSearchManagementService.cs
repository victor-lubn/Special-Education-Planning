using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;

namespace SpecialEducationPlanning
.Domain.Service.Search
{
    public interface IAzureSearchManagementService
    {
        bool DeleteAzureSearch();
        Dictionary<Type, IEnumerable<int>> MergeOrUploadDocuments(List<EntityEntry> entityEntries);
        void DeleteDocuments(Dictionary<Type, IEnumerable<int>> documentTypesAndIds);
        Task<bool> EnsureCreatedAsync(bool deleteAndRecreate);
        Dictionary<Type, IEnumerable<int>> GetDeletedOrUpdatedDocumentsIds(DataContext dbContext, List<EntityEntry> entityEntries);
        IEnumerable<TModel> GetDocuments<TModel, TEntity>(List<TEntity> entities) where TModel : SearchBaseIndexModel where TEntity : ISearchable<int>;
        void MergeOrUploadDocuments<T>(IEnumerable<T> documents);
        Dictionary<Type, List<int>> GetISearchableDocumentsByUserToBeUpdated(DataContext dbContext, Dictionary<Type, IEnumerable<int>> typesAndIds);
        void MergeOrUploadISearchableDocumentsByUser(DataContext dbContext, Dictionary<Type, List<int>> documentIdsWithAclsToUpdate);
    }
}
