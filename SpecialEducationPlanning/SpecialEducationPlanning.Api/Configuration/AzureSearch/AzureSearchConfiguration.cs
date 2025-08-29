using System.Collections.Generic;

namespace SpecialEducationPlanning
.Api.Configuration.AzureSearch
{
    public class AzureSearchConfiguration
    {
        public const string SectionName = "AzureSearch";
        public const string SectionEnabledName = SectionName + ":Enabled";
        public bool Enabled { get; set; }
        public string SearchServiceName { get; set; }
        public string SearchServiceAdminKey { get; set; }
        public string SearchServiceQueryKey { get; set; }
        public bool ManagerEnabled { get; set; }
        public string SearchServiceUrl { get; set; }
        public IEnumerable<string> ReservedChars { get; set; }
        public IEnumerable<string> UnsafeChars { get; set; }
        public IEnumerable<string> SearchOperators { get; set; }
        public string DataSourceConnectionString { get; set; }
        public bool UseIndexer { get; set; }
        public bool LefthandSearchEnabled { get; set; }
        public SearchPropertiesConfiguration BuilderSearchConfig { get; set; }
        public SearchPropertiesConfiguration PlanSearchConfig { get; set; }
        public SearchPropertiesConfiguration VersionSearchConfig { get; set; }
        public SearchPropertiesConfiguration EndUserSearchConfig { get; set; }
        public SearchPropertiesConfiguration UserSearchConfig { get; set; }
        public SearchPropertiesConfiguration ProjectSearchConfig { get; set; }
    }
}



