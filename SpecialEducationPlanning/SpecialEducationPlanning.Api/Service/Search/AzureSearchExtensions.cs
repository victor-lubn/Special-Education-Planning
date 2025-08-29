using Koa.Domain.Search.Page;
using Microsoft.Azure.Search.Models;
using System.Linq;

namespace SpecialEducationPlanning
.Api.Service.Search
{

    internal static class AzureSearchExtensions
    {
        public static string ApplyKoaPageDescriptorFilters<T>(this SearchParameters searchParameters,
            PageDescriptor pageDescriptorSearchModel)
        {
            searchParameters.Skip = pageDescriptorSearchModel.Skip;
            searchParameters.Top = pageDescriptorSearchModel.Take;
            var propertiesNamesFromT = typeof(T).GetProperties().Select(propertyInfo => propertyInfo.Name).ToList();

            if (pageDescriptorSearchModel.Filters.Any(f => !propertiesNamesFromT.Contains(f.Member)))
            {
                //////ERROR!!
                return null;
            }

            var search = "";

            foreach (var filterDescriptor in pageDescriptorSearchModel.Filters)
            {
                search += $"{filterDescriptor.Member}:\"{filterDescriptor.Value}\"&";
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Remove(search.Length - 1);
            }

            return search;
        }

        public static void SelectProperties<T>(this SearchParameters searchParameters)
        {
            searchParameters.Select = typeof(T).GetProperties().Select(propertyInfo => propertyInfo.Name).ToList();
        }
    }
}