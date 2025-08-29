using Koa.Domain.Search.Page;
using Koa.Domain.Search.Sort;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.AzureSearch;
using SpecialEducationPlanning
.Api.Model.AzureSearch;
using SpecialEducationPlanning
.Api.Service.User;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Extensions;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;

namespace SpecialEducationPlanning
.Api.Service.Search
{

    public class AzureSearchService : IAzureSearchService
    {

        private readonly AzureSearchConfiguration azureSearchConfiguration;

        private readonly IHttpContextAccessor contextAccessor;

        private readonly IEnumerable<string> reservedChars;

        private readonly IEnumerable<string> searchOperators;

        private readonly IEnumerable<string> unsafeChars;

        private readonly IUserService userService;

        private readonly ILogger<AzureSearchService> logger;

        private readonly AzureSearchClientPool clientPool;

        public AzureSearchConfiguration AzureSearchConfiguration => azureSearchConfiguration;

        public AzureSearchService(ILogger<AzureSearchService> logger,
            IOptions<AzureSearchConfiguration> configuration, AzureSearchClientPool clientPool,
            IUserService userService,
            IHttpContextAccessor contextAccessor)
        {
            this.userService = userService;
            this.contextAccessor = contextAccessor;
            azureSearchConfiguration = configuration.Value;
            reservedChars = azureSearchConfiguration.ReservedChars;
            unsafeChars = azureSearchConfiguration.UnsafeChars;
            searchOperators = azureSearchConfiguration.SearchOperators;

            this.logger = logger;
            this.clientPool = clientPool;
        }

        #region Methods IAzureSearchService
        /// <summary>
        /// Searches using the OmniSearch
        /// </summary>
        /// <param name="textToSearch"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async Task<Dictionary<Type, HashSet<int>>> OmniSearchSearchAsync(string textToSearch, int AiepId, int skip, int take)
        {
            logger.LogDebug("AzureSearchService called OmniSearchSearchAsync");

            Dictionary<Type, HashSet<int>> entityTypesAndIds = new Dictionary<Type, HashSet<int>>();

            var builderIds = await OmniSearchAsync<OmniSearchBuilderIndexModel>(textToSearch, AiepId, skip, take);
            var planIds = await OmniSearchAsync<OmniSearchPlanIndexModel>(textToSearch, AiepId, skip, take);
            var projectIds = await OmniSearchAsync<OmniSearchProjectIndexModel>(textToSearch, AiepId, skip, take);

            entityTypesAndIds.Add(typeof(Builder), builderIds);
            entityTypesAndIds.Add(typeof(Plan), planIds);
            entityTypesAndIds.Add(typeof(Project), projectIds);

            if (!textToSearch.IsNullOrEmpty())
            {
                var versionIds = await OmniSearchAsync<OmniSearchVersionIndexModel>(textToSearch, AiepId, skip, take);
                var endUserIds = await OmniSearchAsync<OmniSearchEndUserIndexModel>(textToSearch, AiepId, skip, take);
                var userIds = await OmniSearchAsync<OmniSearchUserIndexModel>(textToSearch, AiepId, skip, take);

                entityTypesAndIds.Add(typeof(Domain.Entity.Version), versionIds);
                entityTypesAndIds.Add(typeof(EndUser), endUserIds);
                entityTypesAndIds.Add(typeof(Domain.Entity.User), userIds);
            }

            logger.LogDebug("AzureSearchService end call OmniSearchAsync -> return Dictionary<Type, HashSet<int>>");

            return (entityTypesAndIds);
        }

        #endregion

        #region Methods Private

        private string ConvertTextToSearch(string textToSearch, SearchParameters searchParameters)
        {
            logger.LogDebug("AzureSearchService called ConvertTextToSearch");

            textToSearch = textToSearch.NormaliseSpaces();
            var containsTextToSearch = string.Empty;

            if (textToSearch.Contains(" ") ||
                textToSearch.ToCharArray().Any(c => searchOperators.Select(x => x.ToCharArray().First()).Contains(c)) ||
                textToSearch.ToCharArray().Any(c => reservedChars.Select(x => x.ToCharArray().First()).Contains(c)) ||
                textToSearch.ToCharArray().Any(c => unsafeChars.Select(x => x.ToCharArray().First()).Contains(c)))
            {
                textToSearch = textToSearch.Replace(reservedChars.Select(c => c.ToString()), " ");
                textToSearch = textToSearch.Replace(unsafeChars.Select(c => c.ToString()), " ");
                textToSearch = textToSearch.NormaliseSpaces();
                textToSearch = textToSearch.SubstringWrapper(searchOperators.Select(c => c.ToString()), @"\");

                textToSearch =
                    textToSearch.SubstringWrapper(new List<string> { " " }, "*"); // Adding * before middle spaces

                containsTextToSearch = $"{textToSearch}" + "*";
                searchParameters.QueryType = QueryType.Simple;
                searchParameters.SearchMode = SearchMode.All;
            }
            else
            {
                containsTextToSearch = $"/(.*){textToSearch}(.*)/";
            }

            logger.LogDebug("AzureSearchService end call ConvertTextToSearch -> return String");

            return containsTextToSearch;
        }


        private string CreateSearchString<T>(T model) where T : SearchBaseIndexModel
        {
            logger.LogDebug("AzureSearchService called CreateSearchString");

            var stringBuilder = new StringBuilder();
            foreach (var propertyInfo in model.GetType().GetProperties())
            {
                var attributes = propertyInfo.GetCustomAttributes(true).Select(a => a.GetType().Name);
                if (propertyInfo.GetValue(model).IsNotNull() && attributes.Contains(nameof(IsSearchableAttribute)))
                {
                    stringBuilder.Append(propertyInfo.GetValue(model).ToString() + " ");
                }
            }

            logger.LogDebug("AzureSearchService end call CreateSearchString -> return String");

            return stringBuilder.ToString();
        }

        private List<string> SetSearchFields<T>(T model) where T : SearchBaseIndexModel
        {
            logger.LogDebug("AzureSearchService called SetSearchFields");

            var searchFields = new List<string>();
            var properties = model.GetType().GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(true).Select(a => a.GetType().Name);
                if (property.GetValue(model).IsNotNull() && attributes.Contains(nameof(IsSearchableAttribute)))
                {
                    searchFields.Add(property.Name);
                }
            }

            logger.LogDebug("AzureSearchService end call SetSearchFields -> return List of string");

            return searchFields;
        }

        private List<string> SetOmniSearchPlanSearchFields()
        {
            logger.LogDebug("AzureSearchService called SetOmniSearchPlanSearchFields");

            var model = new OmniSearchPlanIndexModel();
            var searchFields = new List<string>();
            var properties = model.GetType().GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(true).Select(a => a.GetType().Name);
                if (property.Name != nameof(model.EducationerId) && attributes.Contains(nameof(IsSearchableAttribute)))
                {
                    searchFields.Add(property.Name);
                }
            }

            logger.LogDebug("AzureSearchService end call SetOmniSearchPlanSearchFields -> return List of string");

            return searchFields;
        }

        private List<string> SetOmniSearchUserSearchFields()
        {
            logger.LogDebug("AzureSearchService called SetOmniSearchUserSearchFields");

            var model = new OmniSearchUserIndexModel();
            var searchFields = new List<string>();
            var properties = model.GetType().GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(true).Select(a => a.GetType().Name);
                if (property.Name != nameof(model.Id) && attributes.Contains(nameof(IsSearchableAttribute)))
                {
                    searchFields.Add(property.Name);
                }
            }

            logger.LogDebug("AzureSearchService end call SetOmniSearchUserSearchFields -> return List of string");

            return searchFields;
        }


        private List<string> SetOmniSearchBuilderSearchFields()
        {
            logger.LogDebug("AzureSearchService called SetOmniSearchBuilderSearchFields");

            var model = new OmniSearchBuilderIndexModel();
            var searchFields = new List<string>();
            var properties = model.GetType().GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(true).Select(a => a.GetType().Name);
                if (attributes.Contains(nameof(IsSearchableAttribute)))
                {
                    searchFields.Add(property.Name);
                }
            }

            logger.LogDebug("AzureSearchService end call SetOmniSearchBuilderSearchFields -> return List of string");

            return searchFields;
        }

        private List<string> SetOmniSearchProjectSearchFields()
        {
            logger.LogDebug("AzureSearchService called SetOmniSearchProjectSearchFields");

            var model = new OmniSearchProjectIndexModel();
            var searchFields = new List<string>();
            var properties = model.GetType().GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(true).Select(a => a.GetType().Name);
                if (attributes.Contains(nameof(IsSearchableAttribute)))
                {
                    searchFields.Add(property.Name);
                }
            }

            logger.LogDebug("AzureSearchService end call SetOmniSearchProjectSearchFields -> return List of string");

            return searchFields;
        }


        private Dictionary<Type, SearchBaseIndexModel> MapSearchModelToIndexModels(IPageDescriptor model)
        {
            logger.LogDebug("AzureSearchService called MapSearchModelToIndexModels");

            var documents = new Dictionary<Type, SearchBaseIndexModel>();
            string externalCode = null;
            string endUserSurname = null;
            string endUserAddress = null;
            string endUserPostcode = null;
            string planCode = null;
            string planName = null;
            string cadFilePlanId = null;
            string EducationerId = null;
            string planState = null;
            string builderId = null;
            string updatedDate = null;

            foreach (var filter in model.Filters)
            {
                switch (filter.Member)
                {
                    case "versions.externalCode":
                        externalCode = filter.Value;
                        break;
                    case "endUser.surname":
                        endUserSurname = filter.Value;
                        break;
                    case "endUser.address0":
                        endUserAddress = filter.Value;
                        break;
                    case "endUser.postcode":
                        endUserPostcode = filter.Value;
                        break;
                    case "planCode":
                        planCode = filter.Value;
                        break;
                    case "planName":
                        planName = filter.Value;
                        break;
                    case "cadFilePlanId":
                        cadFilePlanId = filter.Value;
                        break;
                    case "EducationerId":
                        EducationerId = filter.Value;
                        break;
                    case "planState":
                        planState = filter.Value;
                        break;
                    case "builderId":
                        builderId = filter.Value;
                        break;
                    case "updatedDate":
                        updatedDate = filter.Value;
                        break;
                    default:
                        break;
                }
            }

            documents.Add(typeof(OmniSearchPlanIndexModel), new OmniSearchPlanIndexModel
            {
                PlanCode = planCode,
                PlanName = planName,
                CadFilePlanId = cadFilePlanId,
                EducationerId = EducationerId,
                PlanState = planState,
                BuilderId = builderId,
                EndUserAddress = endUserAddress,
                EndUserPostcode = endUserPostcode,
                EndUserSurname = endUserSurname,
                UpdatedDate = updatedDate.IsNotNull() ? Convert.ToDateTime(updatedDate) : (DateTime?)null
            });

            if (externalCode.IsNotNull())
            {
                documents.Add(typeof(OmniSearchVersionIndexModel), new OmniSearchVersionIndexModel
                {
                    ExternalCode = externalCode
                });
            }
            //if (endUserAddress.IsNotNull() || endUserSurname.IsNotNull() || endUserPostcode.IsNotNull())
            //{
            //    documents.Add(typeof(OmniSearchEndUserIndexModel), new OmniSearchEndUserIndexModel
            //    {
            //        Surname = endUserSurname,
            //        Address0 = endUserAddress,
            //        Postcode = endUserPostcode
            //    });
            //}

            logger.LogDebug("AzureSearchService end call MapSearchModelToIndexModels -> return Dictionary<Type, SearchBaseIndexModel>");

            return documents;
        }

        private OmniSearchBuilderIndexModel MapSearchModelToBuilderIndexModel(IPageDescriptor model)
        {
            logger.LogDebug("AzureSearchService called MapSearchModelToBuilderIndexModel");

            string accountNumber = null;
            string tradingName = null;
            string name = null;
            string address0 = null;
            string postcode = null;
            string mobileNumber = null;
            string landlineNumber = null;
            //string builderStatus=Enum.GetName(typeof(BuilderStatus), BuilderStatus.None);

            foreach (var filter in model.Filters)
            {
                switch (filter.Member)
                {
                    case "accountNumber":
                        accountNumber = filter.Value;
                        break;
                    case "tradingName":
                        tradingName = filter.Value;
                        break;
                    case "name":
                        name = filter.Value;
                        break;
                    case "address0":
                        address0 = filter.Value;
                        break;
                    case "postcode":
                        postcode = filter.Value;
                        break;
                    case "mobileNumber":
                        mobileNumber = filter.Value;
                        break;
                    case "landlineNumber":
                        landlineNumber = filter.Value;
                        break;
                    //case "sapAccountStatus":
                    //    string sapAccountStatus = filter.Value;
                    //    break;                   
                    default:
                        break;
                }
            }

            logger.LogDebug("AzureSearchService end call MapSearchModelToBuilderIndexModel -> return OmniSearchBuilderIndexModel");

            return new OmniSearchBuilderIndexModel
            {
                AccountNumber = accountNumber,
                TradingName = tradingName,
                Name = name,
                Address0 = address0,
                Postcode = postcode,
                MobileNumber = mobileNumber,
                LandLineNumber = landlineNumber,
                //SAPAccountStatus = sapAccountStatus                
            };
        }

        private OmniSearchProjectIndexModel MapSearchModelToProjectIndexModel(IPageDescriptor model)
        {
            logger.LogDebug("AzureSearchService called MapSearchModelToProjectIndexModel");

            string id = null;
            string codeProject = null;
            string keyName = null;
            string builderName = null;
            string createdDate = null;
            string updatedDate = null;
            string singlePlanProject = null;
            string isArchived = null;

            var otherFilters = new List<string> { "projectName", "projectReference", "builderName", "createdDate", "updatedDate" };
            var isOtherFilters = model.Filters.Where(f => otherFilters.Contains(f.Member)).Any();

            foreach (var filter in model.Filters)
            {
                switch (filter.Member)
                {
                    case "Id":
                        id = filter.Value;
                        break;
                    case "projectName":
                        codeProject = filter.Value;
                        break;
                    case "projectReference":
                        keyName = filter.Value;
                        break;
                    case "builder.tradingName":
                        builderName = filter.Value;
                        break;
                    case "createdDate":
                        createdDate = filter.Value;
                        break;
                    case "updatedDate":
                        updatedDate = filter.Value;
                        break;
                    case "singlePlanProject":
                        singlePlanProject = filter.Value;
                        break;
                    case "isArchived":
                        if ((isOtherFilters && filter.Value == "False") || !isOtherFilters)
                        {
                            isArchived = filter.Value;
                        }
                        break;
                    default:
                        break;
                }
            }

            logger.LogDebug("AzureSearchService end call MapSearchModelToBuilderIndexModel -> return OmniSearchBuilderIndexModel");

            return new OmniSearchProjectIndexModel
            {
                Id = id,
                CodeProject = codeProject,
                KeyName = keyName,
                BuilderName = builderName,
                SinglePlanProject = singlePlanProject,
                IsArchived = isArchived,
                CreatedDate = createdDate.IsNotNull() ? Convert.ToDateTime(createdDate) : (DateTime?)null,
                UpdatedDate = updatedDate.IsNotNull() ? Convert.ToDateTime(updatedDate) : (DateTime?)null
            };
        }


        public async Task<AzureSearchPaginatorIds> GetPlanIdsFilteredAsync(IPageDescriptor model, int AiepId)
        {
            logger.LogDebug("AzureSearchService called GetPlansIdsFilteredAsync");

            var indexModels = MapSearchModelToIndexModels(model);
            var entityTypesAndIds = new Dictionary<Type, HashSet<int>>();
            var planIds = new HashSet<int>();
            var versionIds = new HashSet<int>();
            var endUserIds = new HashSet<int>();

            var skipValue = model.Skip ?? 0;
            var takeValue = model.Take ?? 100;

            var totalCount = 0;
            SortDescriptor sort = null;

            if (indexModels.ContainsKey(typeof(OmniSearchPlanIndexModel)) && !(model.Filters.Count == 1 && model.Filters.First().Member == "versions.externalCode"))
            {
                var planTuple = await GetIdsFilteredAsync((OmniSearchPlanIndexModel)indexModels[typeof(OmniSearchPlanIndexModel)], skipValue, takeValue, AiepId, model);
                planIds = planTuple.Item1;
                totalCount += planTuple.Item2;
                sort = planTuple.Item3;
            }

            if (indexModels.ContainsKey(typeof(OmniSearchVersionIndexModel)))
            {
                var versionTuple = await GetIdsFilteredAsync((OmniSearchVersionIndexModel)indexModels[typeof(OmniSearchVersionIndexModel)], skipValue, takeValue, AiepId, model);
                versionIds = versionTuple.Item1;
                totalCount += versionTuple.Item2;
                sort = versionTuple.Item3;
            }

            //if (indexModels.ContainsKey(typeof(OmniSearchEndUserIndexModel)))
            //{
            //    var endUserTuple = await GetIdsFilteredAsync((OmniSearchEndUserIndexModel)indexModels[typeof(OmniSearchEndUserIndexModel)], skipValue, takeValue, AiepId, model);
            //    endUserIds = endUserTuple.Item1;
            //    totalCount += endUserTuple.Item2;
            //}

            entityTypesAndIds.Add(typeof(Plan), planIds);
            entityTypesAndIds.Add(typeof(Domain.Entity.Version), versionIds);
            entityTypesAndIds.Add(typeof(EndUser), endUserIds);
            entityTypesAndIds.Add(typeof(Domain.Entity.User), new HashSet<int>());
            entityTypesAndIds.Add(typeof(Builder), new HashSet<int>());

            logger.LogDebug("AzureSearchService end call GetPlansIdsFilteredAsync -> return AzureSearchPaginatorIds");

            return new AzureSearchPaginatorIds { PlanFilteredIds = entityTypesAndIds, TotalCount = totalCount, Sort = sort };
        }

        public async Task<AzureSearchPaginatorIds> GetProjectIdsFilteredAsync(IPageDescriptor model, int AiepId)
        {
            logger.LogDebug("AzureSearchService called GetPlansIdsFilteredAsync");

            var projectIndexModel = MapSearchModelToProjectIndexModel(model);
            var searchParameters = new SearchParameters
            {
                Skip = model.Skip,
                Top = model.Take,
                IncludeTotalResultCount = true,
                QueryType = QueryType.Full,
                SearchFields = SetSearchFields(projectIndexModel)
            };

            searchParameters.Select = new List<string> { nameof(SearchBaseIndexModel.Id) };

            var fullAclAccess =
                userService.GetUserFullAclAccess((ClaimsIdentity)contextAccessor.HttpContext.User.Identity);

            var userId = -1;
            if (!fullAclAccess)
            {
                userId = userService.GetUserId((ClaimsIdentity)contextAccessor.HttpContext.User.Identity);
            }

            searchParameters = AddFilters(projectIndexModel, fullAclAccess, userId, AiepId, searchParameters);

            if (model.Sorts.Count > 0)
            {
                var direction = model.Sorts.First().Direction == SortDirection.Descending ? "desc" : "asc";
                searchParameters.OrderBy = new List<string> { $"" + model.Sorts.First().Member + " " + direction };
            }

            var containsTextToSearch = ConvertTextToSearch(CreateSearchString(projectIndexModel), searchParameters);
            var searchIndexClient = GetSearchIndexClient<OmniSearchProjectIndexModel>();

            logger.LogDebug("AzureSearchService GetPlansIdsFilteredAsync call SearchAsync");

            var searchResults = await searchIndexClient.Documents.SearchAsync<OmniSearchProjectIndexModel>(containsTextToSearch,
                    searchParameters, new SearchRequestOptions());

            logger.LogDebug("AzureSearchService GetPlansIdsFilteredAsync end call SearchAsync");

            var documents = searchResults.Results.Select(r => r.Document);
            var ids = new HashSet<int>();
            ids.AddRange(documents.Select(d => Int32.Parse(d.Id)).ToList());

            SortDescriptor sort = null;
            if (model.Sorts.Count > 0)
            {
                sort = new SortDescriptor { Member = model.Sorts.First().Member, Direction = model.Sorts.FirstOrDefault().Direction };
            }

            logger.LogDebug("AzureSearchService end call GetPlansIdsFilteredAsync -> return AzureSearchPaginatorIds");

            return new AzureSearchPaginatorIds { ProjectFilteredIds = ids, TotalCount = Convert.ToInt32(searchResults.Count), Sort = sort };
        }

        private SearchParameters AddFilters<T>(T model, bool fullAclAccess, int userId, int AiepId, SearchParameters searchParameters) where T : SearchBaseIndexModel
        {
            logger.LogDebug("AzureSearchService called AddFilters");

            var filters = new List<string>();

            if (!fullAclAccess)
            {
                filters.Add($"{nameof(model.Acls)}/any(acl: acl eq {userId})");
            }
            if (AiepId != -1 && AiepId.IsNotNull())
            {
                filters.Add($"{nameof(model.AiepIds)}/any(id: id eq {AiepId})");
            }

            if (model.GetType() == typeof(OmniSearchPlanIndexModel))
            {
                var planModel = model as OmniSearchPlanIndexModel;

                if (!string.IsNullOrWhiteSpace(planModel.EducationerId))
                {
                    filters.Add($"EducationerId eq '{planModel.EducationerId}'");
                }
                if (planModel.PlanState.IsNotNull())
                {
                    filters.Add($"PlanState eq 'Archived'");
                }
                else
                {
                    filters.Add($"PlanState eq 'Active'");
                }

                if (planModel.UpdatedDate.IsNotNull())
                {
                    //The standard format specifier "O" or "o" represents a custom date and time format string using a pattern
                    //that retains time zone information and outputs an ISO 8601-compliant result string
                    filters.Add($"UpdatedDate ge {planModel.UpdatedDate.Value.ToString("o")}");
                }

                if (!planModel.BuilderId.IsNullOrEmpty())
                {
                    filters.Add($"BuilderId eq '{planModel.BuilderId}'");
                }
                else if (planModel.BuilderId.IsNotNull())
                {
                    filters.Add($"BuilderId eq null");
                }
            }

            if (model.GetType() == typeof(OmniSearchProjectIndexModel))
            {
                var projectModel = model as OmniSearchProjectIndexModel;

                if (!string.IsNullOrWhiteSpace(projectModel.Id))
                {
                    filters.Add($"Id eq '{projectModel.Id}'");
                }

                if (projectModel.SinglePlanProject.IsNotNull())
                {
                    filters.Add($"SinglePlanProject eq '{projectModel.SinglePlanProject}'");
                }

                if (projectModel.CreatedDate.IsNotNull())
                {
                    var createdDate = projectModel.CreatedDate.Value;
                    var createdDateNextDay = projectModel.CreatedDate.Value.AddDays(1);
                    filters.Add($"CreatedDate ge {createdDate.ToString("o")}{createdDate.ToString("zzz")}");
                    filters.Add($"CreatedDate le {createdDateNextDay.ToString("o")}{createdDateNextDay.ToString("zzz")}");
                }

                if (projectModel.UpdatedDate.IsNotNull())
                {
                    var updatedDate = projectModel.UpdatedDate.Value;
                    var updatedDateNextDay = projectModel.UpdatedDate.Value.AddDays(1);
                    filters.Add($"UpdatedDate ge {updatedDate.ToString("o")}{updatedDate.ToString("zzz")}");
                    filters.Add($"UpdatedDate le {updatedDateNextDay.ToString("o")}{updatedDateNextDay.ToString("zzz")}");
                }

                if (projectModel.IsArchived.IsNotNull())
                {
                    filters.Add($"IsArchived eq '{projectModel.IsArchived}'");
                }
            }

            if (filters.IsNotNull())
            {
                searchParameters.Filter = ConcatenateFilters(filters);
            }

            logger.LogDebug("AzureSearchService end call AddFilters -> return SearchParameters");
            return searchParameters;
        }

        private static string ConcatenateFilters(List<string> filters)
        {
            return filters.Join(" and ");
        }

        private async Task<Tuple<HashSet<int>, int, SortDescriptor>> GetIdsFilteredAsync<T>(T model, int skip, int take, int AiepId, IPageDescriptor pageDescriptor) where T : SearchBaseIndexModel
        {
            logger.LogDebug("AzureSearchService called GetIdsFilteredAsync");

            var searchParameters = new SearchParameters
            {
                Skip = skip,
                Top = take,
                IncludeTotalResultCount = true,
                QueryType = QueryType.Full,
                SearchFields = SetSearchFields(model)
            };

            searchParameters.Select = new List<string> { nameof(model.Id) };

            var fullAclAccess =
                userService.GetUserFullAclAccess((ClaimsIdentity)contextAccessor.HttpContext.User.Identity);

            var userId = -1;
            if (!fullAclAccess)
            {
                userId = userService.GetUserId((ClaimsIdentity)contextAccessor.HttpContext.User.Identity);
            }

            searchParameters = AddFilters(model, fullAclAccess, userId, AiepId, searchParameters);

            if (pageDescriptor.Sorts.Count > 0)
            {
                var direction = pageDescriptor.Sorts.First().Direction == SortDirection.Descending ? "desc" : "asc";
                var orderBy = pageDescriptor.Sorts.First().Member.Replace(".", string.Empty);
                searchParameters.OrderBy = new List<string> { $"" + orderBy + " " + direction };
            }

            var containsTextToSearch = ConvertTextToSearch(CreateSearchString(model), searchParameters);
            var searchIndexClient = GetSearchIndexClient<T>();

            logger.LogDebug("AzureSearchService GetIdsFilteredAsync call Documents.SearchAsync");

            var searchResults = await searchIndexClient.Documents.SearchAsync<T>(containsTextToSearch,
                    searchParameters, new SearchRequestOptions());

            logger.LogDebug("AzureSearchService GetIdsFilteredAsync end call Documents.SearchAsync");

            var documents = searchResults.Results.Select(r => r.Document);
            var ids = new HashSet<int>();
            ids.AddRange(documents.Select(d => Int32.Parse(d.Id)).ToList());

            logger.LogDebug("AzureSearchService end call GetIdsFilteredAsync -> return Tuple<HashSet<int>, int, SortDescriptor>");

            return new Tuple<HashSet<int>, int, SortDescriptor>(ids, Convert.ToInt32(searchResults.Count), pageDescriptor.Sorts.FirstOrDefault());
        }


        public async Task<AzureSearchPaginatorIds> GetBuilderIdsFilteredAsync<T>(IPageDescriptor model, int AiepId) where T : SearchBaseIndexModel
        {
            logger.LogDebug("AzureSearchService called GetBuilderIdsFilteredAsync");

            var indexModel = new OmniSearchBuilderIndexModel();
            foreach (var filter in model.Filters)
            {
                if (filter.Member.ToLower() == nameof(indexModel.MobileNumber).ToLower() || filter.Member.ToLower() == nameof(indexModel.LandLineNumber).ToLower())
                {
                    filter.Value = filter.Value.NormaliseNumber();
                }

                if (filter.Member.ToLower() == nameof(indexModel.Postcode).ToLower())
                {
                    filter.Value = filter.Value.NormalisePostcode();
                }
            }

            var builderIndexModel = MapSearchModelToBuilderIndexModel(model);
            var searchParameters = new SearchParameters
            {
                Skip = model.Skip,
                Top = model.Take,
                IncludeTotalResultCount = true,
                QueryType = QueryType.Full,
                SearchFields = SetSearchFields(builderIndexModel)
            };

            searchParameters.Select = new List<string> { nameof(SearchBaseIndexModel.Id) };

            var fullAclAccess =
                userService.GetUserFullAclAccess((ClaimsIdentity)contextAccessor.HttpContext.User.Identity);

            var userId = -1;
            if (!fullAclAccess)
            {
                userId = userService.GetUserId((ClaimsIdentity)contextAccessor.HttpContext.User.Identity);
            }
            searchParameters = AddFilters(new OmniSearchBuilderIndexModel { }, fullAclAccess, userId, AiepId, searchParameters);

            // searchParameters.OrderBy = new List<string> { $"{nameof(SearchBaseIndexModel.UpdatedDate)} desc" };

            if (model.Sorts.Count > 0)
            {
                var direction = model.Sorts.First().Direction == SortDirection.Descending ? "desc" : "asc";
                searchParameters.OrderBy = new List<string> { $"" + model.Sorts.First().Member + " " + direction };
            }

            var containsTextToSearch = ConvertTextToSearch(CreateSearchString(builderIndexModel), searchParameters);
            var searchIndexClient = GetSearchIndexClient<T>();

            logger.LogDebug("AzureSearchService GetBuilderIdsFilteredAsync call SearchAsync");

            var searchResults = await searchIndexClient.Documents.SearchAsync<T>(containsTextToSearch,
                    searchParameters, new SearchRequestOptions());

            logger.LogDebug("AzureSearchService GetBuilderIdsFilteredAsync end call SearchAsync");

            var documents = searchResults.Results.Select(r => r.Document);
            var ids = new HashSet<int>();
            ids.AddRange(documents.Select(d => Int32.Parse(d.Id)).ToList());

            SortDescriptor sort = null;
            if (model.Sorts.Count > 0)
            {
                sort = new SortDescriptor { Member = model.Sorts.First().Member, Direction = model.Sorts.FirstOrDefault().Direction };
            }

            logger.LogDebug("AzureSearchService end call GetBuilderIdsFilteredAsync -> return AzureSearchPaginatorIds");

            return new AzureSearchPaginatorIds { BuilderFilterdIds = ids, TotalCount = Convert.ToInt32(searchResults.Count), Sort = sort };
        }


        private SearchParameters AddOmniSearchFilters(int AiepId, int userId, bool fullAclAccess, SearchParameters searchParameters)
        {
            logger.LogDebug("AzureSearchService called AddOmniSearchFilters");

            var filters = new List<string>();

            if (!fullAclAccess)
            {
                filters.Add($"{nameof(SearchBaseIndexModel.Acls)}/any(acl: acl eq {userId})");
            }

            if (AiepId.IsNotNull() && AiepId != -1)
            {
                filters.Add($"{nameof(SearchBaseIndexModel.AiepIds)}/any(id: id eq {AiepId})");
            }

            if (filters.IsNotNull())
            {
                searchParameters.Filter = ConcatenateFilters(filters);
            }

            logger.LogDebug("AzureSearchService end call AddOmniSearchFilters -> return SearchParameters");

            return searchParameters;
        }

        private async Task<HashSet<int>> OmniSearchAsync<T>(string textToSearch, int AiepId, int skip, int take) where T : SearchBaseIndexModel
        {
            logger.LogDebug("AzureSearchService called OmniSearchAsync");
            var searchParameters = new SearchParameters
            { Skip = skip, Top = take, IncludeTotalResultCount = true, QueryType = QueryType.Full };

            if (typeof(T) == typeof(OmniSearchBuilderIndexModel))
            {
                textToSearch = ConvertBuildrStatusSearchText(textToSearch);
                searchParameters.SearchFields = SetOmniSearchBuilderSearchFields();
            }

            if (typeof(T) == typeof(OmniSearchPlanIndexModel))
            {
                searchParameters.SearchFields = SetOmniSearchPlanSearchFields();
            }

            if (typeof(T) == typeof(OmniSearchProjectIndexModel))
            {
                searchParameters.SearchFields = SetOmniSearchProjectSearchFields();
            }

            if (typeof(T) == typeof(OmniSearchUserIndexModel))
            {
                searchParameters.SearchFields = SetOmniSearchUserSearchFields();
            }

            searchParameters.Select = new List<string> { nameof(SearchBaseIndexModel.Id) };

            var fullAclAccess =
                userService.GetUserFullAclAccess((ClaimsIdentity)contextAccessor.HttpContext.User.Identity);

            var userId = -1;
            if (!fullAclAccess)
            {
                userId = userService.GetUserId((ClaimsIdentity)contextAccessor.HttpContext.User.Identity);
            }

            searchParameters = AddOmniSearchFilters(AiepId, userId, fullAclAccess, searchParameters);

            if (typeof(T) == typeof(OmniSearchProjectIndexModel))
            {
                if (string.IsNullOrEmpty(searchParameters.Filter))
                {
                    searchParameters.Filter = "IsArchived eq 'False'";
                }
                else
                {
                    searchParameters.Filter += " and IsArchived eq 'False'";
                }
            }

            searchParameters.OrderBy = new List<string> { $"{nameof(SearchBaseIndexModel.UpdatedDate)} desc" };

            var containsTextToSearch = ConvertTextToSearch(textToSearch, searchParameters);
            var searchIndexClient = GetSearchIndexClient<T>();

            logger.LogDebug("AzureSearchService AddOmniSearchFilters call SearchAsync");

            var searchResults = await searchIndexClient.Documents.SearchAsync<T>(containsTextToSearch,
                    searchParameters, new SearchRequestOptions());

            logger.LogDebug("AzureSearchService AddOmniSearchFilters end call SearchAsync");

            var documents = searchResults.Results.Select(r => r.Document);
            var ids = new HashSet<int>();
            ids.AddRange(documents.Select(d => Int32.Parse(d.Id)).ToList());

            logger.LogDebug("AzureSearchService end call AddOmniSearchFilters -> return HashSet<int>");

            return (ids);
        }

        private string ConvertBuildrStatusSearchText(string textToSearch)
        {
            if (!textToSearch.IsNullOrEmpty())
            {
                var builderStatus = Enum.GetNames(typeof(BuilderStatus)).FirstOrDefault(s => s.Equals(textToSearch.Trim(), StringComparison.OrdinalIgnoreCase));

                if (!builderStatus.IsNullOrEmpty())
                {
                    textToSearch = ((Enum.Parse(typeof(BuilderStatus), builderStatus))).ToString();
                }
            }

            return textToSearch;
        }

        private ISearchIndexClient GetSearchIndexClient<T>()
        {
            logger.LogDebug("AzureSearchService called GetSearchIndexClient");

            switch (typeof(T).Name)
            {
                case nameof(OmniSearchBuilderIndexModel):

                    logger.LogDebug("AzureSearchService end call GetSearchIndexClient case Builder");

                    return clientPool.SearchBuilderIndexClient;
                case nameof(OmniSearchPlanIndexModel):

                    logger.LogDebug("AzureSearchService end call GetSearchIndexClient case Plan");

                    return clientPool.SearchPlanIndexClient;
                case nameof(OmniSearchVersionIndexModel):

                    logger.LogDebug("AzureSearchService end call GetSearchIndexClient case Version");

                    return clientPool.SearchVersionIndexClient;
                case nameof(OmniSearchEndUserIndexModel):

                    logger.LogDebug("AzureSearchService end call GetSearchIndexClient case EndUser");

                    return clientPool.SearchEndUserIndexClient;
                case nameof(OmniSearchUserIndexModel):

                    logger.LogDebug("AzureSearchService end call GetSearchIndexClient case User");

                    return clientPool.SearchUserIndexClient;
                case nameof(OmniSearchProjectIndexModel):

                    logger.LogDebug("AzureSearchService end call GetSearchIndexClient case Project");

                    return clientPool.SearchProjectIndexClient;
                default:

                    logger.LogDebug("AzureSearchService end call GetSearchIndexClient case Null");

                    return null;
            }
        }


        #endregion

    }

}

