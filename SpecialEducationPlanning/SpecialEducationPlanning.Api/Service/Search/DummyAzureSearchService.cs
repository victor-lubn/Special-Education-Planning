using Koa.Domain.Search.Page;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Model.AzureSearch;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;

namespace SpecialEducationPlanning
.Api.Service.Search
{
    public class DummyAzureSearchService : IAzureSearchService
    {
        public Task<AzureSearchPaginatorIds> GetBuilderIdsFilteredAsync<T>(IPageDescriptor model, int AiepId) where T : SearchBaseIndexModel
        {
            return Task.FromResult(new AzureSearchPaginatorIds { });
        }

        public Task<AzureSearchPaginatorIds> GetPlanIdsFilteredAsync(IPageDescriptor model, int AiepId)
        {
            return Task.FromResult(new AzureSearchPaginatorIds { });
        }

        public Task<AzureSearchPaginatorIds> GetProjectIdsFilteredAsync(IPageDescriptor model, int AiepId)
        {
            return Task.FromResult(new AzureSearchPaginatorIds { });
        }

        public Task<Dictionary<Type, HashSet<int>>> OmniSearchSearchAsync(string textToSearch, int AiepId, int skip, int take)
        {
            return Task.FromResult(new Dictionary<Type, HashSet<int>>());
        }
    }
}

