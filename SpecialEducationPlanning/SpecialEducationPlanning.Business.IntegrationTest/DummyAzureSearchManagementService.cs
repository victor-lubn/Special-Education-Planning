using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;
using SpecialEducationPlanning
.Domain.Service.Search;

namespace SpecialEducationPlanning
.Business.IntegrationTest
{
    public class DummyAzureSearchManagementService : IAzureSearchManagementService
    {
        public bool DeleteAzureSearch()
        {
            return false;
        }

        public void DeleteDocuments(Dictionary<Type, IEnumerable<int>> typesAndIds)
        {

        }

        public Task<bool> EnsureCreatedAsync(bool deleteAndRecreate)
        {
            return Task.FromResult(false);
        }

        public Dictionary<Type, IEnumerable<int>> GetDeletedOrUpdatedDocumentsIds(DataContext dbContext, List<EntityEntry> entityEntries)
        {
            return new Dictionary<Type, IEnumerable<int>>();
        }

        public Dictionary<Type, List<int>> GetISearchableDocumentsByUserToBeUpdated(DataContext dbContext, Dictionary<Type, IEnumerable<int>> typesAndIds)
        {
            return new Dictionary<Type, List<int>>();
        }

        public IEnumerable<TModel> GetDocuments<TModel, TEntity>(List<TEntity> entities)
            where TModel : SearchBaseIndexModel
            where TEntity : ISearchable<int>
        {
            return new List<TModel>();
        }

        public void MergeOrUploadISearchableDocumentsByUser(DataContext dbContext, Dictionary<Type, List<int>> documentIdsWithAclsToUpdate)
        {

        }

        public void MergeOrUploadDocuments<T>(IEnumerable<T> documents)
        {

        }

        Dictionary<Type, IEnumerable<int>> IAzureSearchManagementService.MergeOrUploadDocuments(List<EntityEntry> entityEntries)
        {
            return new Dictionary<Type, IEnumerable<int>>();
        }
    }
}
