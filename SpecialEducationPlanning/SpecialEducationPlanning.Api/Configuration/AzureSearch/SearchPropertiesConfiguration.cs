namespace SpecialEducationPlanning
.Api.Configuration.AzureSearch
{
    public class SearchPropertiesConfiguration
    {
        public string IndexName { get; set; }
        public string IndexerName { get; set; }
        public string DataSourceTableOrView { get; set; }
        public string DataSourceName { get; set; }
    }
}
