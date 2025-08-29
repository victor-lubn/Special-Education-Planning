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
    public interface IAzureSearchService
    {
        Task<Dictionary<Type, HashSet<int>>> OmniSearchSearchAsync(string textToSearch, int AiepId, int skip, int take);
        Task<AzureSearchPaginatorIds> GetBuilderIdsFilteredAsync<T>(IPageDescriptor model, int AiepId) where T : SearchBaseIndexModel;
        Task<AzureSearchPaginatorIds> GetPlanIdsFilteredAsync(IPageDescriptor model, int AiepId);
        Task<AzureSearchPaginatorIds> GetProjectIdsFilteredAsync(IPageDescriptor model, int AiepId);
    }
}

