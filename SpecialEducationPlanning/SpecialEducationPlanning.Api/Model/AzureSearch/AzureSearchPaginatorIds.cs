using Koa.Domain.Search.Sort;
using System;
using System.Collections.Generic;

namespace SpecialEducationPlanning
.Api.Model.AzureSearch
{
    public class AzureSearchPaginatorIds
    {
        public Dictionary<Type, HashSet<int>> PlanFilteredIds { get; set; }
        public HashSet<int> ProjectFilteredIds { get; set; }
        public HashSet<int> BuilderFilterdIds { get; set; }
        public int TotalCount { get; set; }
        public SortDescriptor Sort { get; set; }
    }
}
