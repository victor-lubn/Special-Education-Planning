
using Microsoft.Azure.Search;
using Microsoft.Extensions.Options;

using SpecialEducationPlanning
.Api.Configuration.AzureSearch;

namespace SpecialEducationPlanning
.Api.Service.Search
{
    public class AzureSearchClientPool
    {

        public ISearchIndexClient SearchBuilderIndexClient { get; }

        public ISearchIndexClient SearchPlanIndexClient { get; }

        public ISearchIndexClient SearchVersionIndexClient { get; }

        public ISearchIndexClient SearchEndUserIndexClient { get; }

        public ISearchIndexClient SearchUserIndexClient { get; }

        public ISearchIndexClient SearchProjectIndexClient { get; }

        public AzureSearchClientPool(IOptions<AzureSearchConfiguration> configuration)
        {
            var azureSearchConfiguration = configuration.Value;
            var searchServiceName = azureSearchConfiguration.SearchServiceName;
            var searchServiceQueryApiKey = azureSearchConfiguration.SearchServiceQueryKey;
            var omniSearchBuilderIndexName = azureSearchConfiguration.BuilderSearchConfig.IndexName;
            var omniSearchPlanIndexName = azureSearchConfiguration.PlanSearchConfig.IndexName;
            var omniSearchVersionIndexName = azureSearchConfiguration.VersionSearchConfig.IndexName;
            var omniSearchEndUserIndexName = azureSearchConfiguration.EndUserSearchConfig.IndexName;
            var omniSearchUserIndexName = azureSearchConfiguration.UserSearchConfig.IndexName;
            var omniSearchProjectIndexName = azureSearchConfiguration.ProjectSearchConfig.IndexName;

            this.SearchBuilderIndexClient = new SearchIndexClient(searchServiceName, omniSearchBuilderIndexName,
                new SearchCredentials(searchServiceQueryApiKey));

            this.SearchPlanIndexClient = new SearchIndexClient(searchServiceName, omniSearchPlanIndexName,
                new SearchCredentials(searchServiceQueryApiKey));

            this.SearchVersionIndexClient = new SearchIndexClient(searchServiceName, omniSearchVersionIndexName,
                new SearchCredentials(searchServiceQueryApiKey));

            this.SearchEndUserIndexClient = new SearchIndexClient(searchServiceName, omniSearchEndUserIndexName,
                new SearchCredentials(searchServiceQueryApiKey));

            this.SearchUserIndexClient = new SearchIndexClient(searchServiceName, omniSearchUserIndexName,
                new SearchCredentials(searchServiceQueryApiKey));
            
            this.SearchProjectIndexClient = new SearchIndexClient(searchServiceName, omniSearchProjectIndexName,
                new SearchCredentials(searchServiceQueryApiKey));
        }
    }
}
