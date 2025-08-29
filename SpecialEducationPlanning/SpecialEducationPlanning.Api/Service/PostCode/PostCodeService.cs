using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SpecialEducationPlanning
.Api.Configuration.PostCode;
using SpecialEducationPlanning
.Api.Model.PostCodeServiceModel;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Model.PostCode;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.Strategy.PostCode;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Service.PostCode
{
    /// <summary>
    /// </summary>
    public class PostCodeService : IPostCodeService
    {
        private readonly string authToken;
        private readonly string baseUrl;
        private readonly string country;
        private readonly string dataset;
        private readonly HttpClient httpClient;
        private readonly ILogger<PostCodeService> logger;
        private readonly IPostCodeStrategy postCodeStrategy;
        private readonly string searchAddressEndpoint;
        private readonly string take;
        private const string NO_POSTCODE = "N/P";

        /// <summary>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="postCodeStrategy"></param>
        /// <param name="httpClient"></param>
        public PostCodeService(PostCodeConfiguration options,
            ILogger<PostCodeService> logger, IPostCodeStrategy postCodeStrategy, HttpClient httpClient)
        {
            baseUrl = options.BaseUrl;
            authToken = options.AuthToken;
            country = options.CountryCode;
            dataset = options.Dataset;
            take = options.Take;
            searchAddressEndpoint = options.SearchAddressEndpoint;

            this.logger = logger;
            this.postCodeStrategy = postCodeStrategy;
            this.httpClient = httpClient;
            this.httpClient.DefaultRequestHeaders.Add("Auth-Token", authToken);
        }


        #region Public Methods

        #region SearchAddress

        /// <summary>
        ///     Searches for address using Experian
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<RepositoryResponse<IEnumerable<AddressSearchResultModel>>> SearchAddressAsync(string query)
        {
            var repositoryResponse = new RepositoryResponse<IEnumerable<AddressSearchResultModel>>();
            logger.LogDebug("PostCodeService called SearchAddressAsync");

            if (string.IsNullOrWhiteSpace(query))
            {
                repositoryResponse.ErrorList.Add(ErrorCode.NullOrWhitespace.GetDescription());
                logger.LogDebug("PostCodeService end call SearchAddressAsync -> argument null or whitespace");
                repositoryResponse.AddError(ErrorCode.NullOrWhitespace, "Null or whitespace");
                return repositoryResponse;
            }
            query = query.Replace(NO_POSTCODE, string.Empty);
            if (string.IsNullOrWhiteSpace(query))
            {
                logger.LogDebug("PostCodeService end call SearchAddressAsync. Value passed is N/P -> return RepositoryResponse");
                repositoryResponse.Content = new List<AddressSearchResultModel>();
                return repositoryResponse;
            }

            string responseBody = null;

            try
            {
                responseBody = await SearchAddress(query);
            }
            catch (Exception ex)
            {
                logger.LogError("PostCodeService end call SearchAddressAsync -> Communication exception");

                throw new CommunicationException("Postcode service error: ", ex);
            }

            if (responseBody.IsNullOrEmpty())
            {
                repositoryResponse.ErrorList.Add(ErrorCode.NoResults.GetDescription());
                logger.LogDebug("PostCodeService end call SearchAddressAsync. Response body null or empty -> return RepositoryResponse");

                return repositoryResponse;
            }

            var response = JsonConvert.DeserializeObject<AddressSearchResponseModel>(responseBody,
                new JsonSerializerSettings
                    { NullValueHandling = NullValueHandling.Ignore, DateParseHandling = DateParseHandling.None });

            repositoryResponse.Content = new Collection<AddressSearchResultModel>(response.Results);
            logger.LogDebug("PostCodeService end call SearchAddressAsync -> return RepositoryResponse");
            return repositoryResponse;
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public RepositoryResponse<string> GetPostCode(string postcode)
        {
            logger.LogDebug("PostCodeService called GetPostCode");

            return postCodeStrategy.GetPostCode(postcode);
        }

        /// <summary>
        ///     Check if PostCode is valid
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns>Returns true is valid</returns>
        public bool IsValidPostcode(string postcode)
        {
            logger.LogDebug("PostCodeService called IsValidPostcode");

            return postCodeStrategy.IsValidPostcode(postcode);
        }

        /// <summary>
        ///     Transforming to Uppercase and setting the correct spacing in between.
        ///     This is used also for SAP.  Example: postcode like w1D 1Nn to W1D 1NN.
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public string RepresentPostcode(string postcode)
        {
            logger.LogDebug("PostCodeService called RepresentPostcode");

            return postCodeStrategy.RepresentPostcode(postcode);
        }


        /// <summary>
        ///     Replacing all spaces for empty and setting to Uppercase.
        ///     This is used for TDP DDBB.  Example: postcode like w1D 1Nn to W1D1NN.
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public string NormalisePostcode(string postcode)
        {
            logger.LogDebug("PostCodeService called NormalisePostcode");

            return postCodeStrategy.NormalisePostcode(postcode);
        }


        /// <summary>
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<RepositoryResponse<AddressModel>> GetAddressByUriAsync(string uri)
        {
            var repositoryResponse = new RepositoryResponse<AddressModel>();
            if (string.IsNullOrWhiteSpace(uri))
            {
                repositoryResponse.ErrorList.Add(ErrorCode.NullOrWhitespace.GetDescription());
                logger.LogDebug("PostCodeService end call GetAddressByUriAsync -> URI null or whitespace");
                return repositoryResponse;
            }

            string responseBody = null;
            try
            {
                responseBody = await GetAddressByUri(uri);
            }
            catch (Exception ex)
            {
                logger.LogError("PostCodeService end call GetAddressByUriAsync -> Communication exception");

                throw new CommunicationException("Postcode service error: ", ex);
            }

            if (responseBody.IsNullOrEmpty())
            {
                repositoryResponse.ErrorList.Add(ErrorCode.NoResults.GetDescription());
                logger.LogDebug(
                    "PostCodeService end call GetAddressByUriAsync. Response body null or empty -> return RepositoryResponse");
                return repositoryResponse;
            }

            var response = JsonConvert.DeserializeObject<AddressResponseModel>(responseBody,
                new JsonSerializerSettings
                    { NullValueHandling = NullValueHandling.Ignore, DateParseHandling = DateParseHandling.None });
            var address = FlattenAddress(response);
            repositoryResponse.Content = FormatAddress(address);

            logger.LogDebug("PostCodeService end call GetAddressByUriAsync -> return RepositoryResponse");

            return repositoryResponse;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private AddressModel FormatAddress(AddressModel address)
        {
            logger.LogDebug("PostCodeService called FormatAddress");
            address.AddressLine3 = ConcatenateAddressLine3(address);
            address.Locality = string.Empty;
            address.Province = string.Empty;
            logger.LogDebug("PostCodeService end call FormatAddress -> return AddressModel");
            return address;
        }

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private string ConcatenateAddressLine3(AddressModel address)
        {
            logger.LogDebug("PostCodeService called ConcatenateAddressLine3");
            var addressLines = new Collection<string>();

            if (!string.IsNullOrWhiteSpace(address.AddressLine3))
                addressLines.Add(address.AddressLine3);
            if (!string.IsNullOrWhiteSpace(address.Locality))
                addressLines.Add(address.Locality);
            if (!string.IsNullOrWhiteSpace(address.Province))
                addressLines.Add(address.Province);

            logger.LogDebug("PostCodeService end call ConcatenateAddressLine3 -> return string");
            return addressLines.Any() ? string.Join(", ", addressLines) : string.Empty;
        }

        /// <summary>
        ///     Flattens list of address model objects into a single object
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private AddressModel FlattenAddress(AddressResponseModel response)
        {
            logger.LogDebug("PostCodeService called FlattenAddress");
            var flattenedAddress = new AddressModel();
            foreach (var address in response.Address)
            foreach (var property in address.GetType().GetProperties())
                if (property.GetValue(address).IsNotNull())
                    property.SetValue(flattenedAddress, property.GetValue(address));
            logger.LogDebug("PostCodeService end call of FlattenAddress");
            return flattenedAddress;
        }

        /// <summary>
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private async Task<string> SearchAddress(string query)
        {
            logger.LogDebug("PostCodeService called SearchAddress");

            var queryParameters = new Dictionary<string, string>
            {
                { nameof(query), query },
                { nameof(country), country },
                { nameof(dataset), dataset },
                { nameof(take), take }
            };

            queryParameters = queryParameters.Where(qp => !string.IsNullOrWhiteSpace(qp.Value)).ToDictionary(qp => qp.Key, qp => qp.Value);
            var uri = GetEndpoint(baseUrl, searchAddressEndpoint, queryParameters);
            var responseBody = await GetHttpRequestAsync(uri.AbsoluteUri);

            logger.LogDebug("PostCodeService end call SearchAddress -> return String");

            return responseBody;
        }

        /// <summary>
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="path"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        private Uri GetEndpoint(string baseUrl, string path, Dictionary<string, string> queryParameters)
        {
            logger.LogDebug("PostCodeService called GetEndpoint");

            var uriBuilder = new UriBuilder(baseUrl);
            uriBuilder.Path = path;

            if (queryParameters != null && queryParameters.Count > 0)
            {
                var stringBuilder = new StringBuilder();
                foreach (var param in queryParameters) stringBuilder.AppendFormat("{0}={1}&", param.Key, param.Value);

                uriBuilder.Query = stringBuilder.ToString().TrimEnd('&');
            }

            logger.LogDebug("PostCodeService end call GetEndpoint -> return Uri");

            return uriBuilder.Uri;
        }

        /// <summary>
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private async Task<string> GetAddressByUri(string uri)
        {
            logger.LogDebug("PostCodeService called GetAddressByUri");

            var response = await GetHttpRequestAsync(uri);

            logger.LogDebug("PostCodeService end call GetAddressByUri -> return String");

            return response;
        }

        /// <summary>
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        private async Task<string> GetHttpRequestAsync(string requestUri)
        {
            logger.LogDebug("PostCodeService called GetHttpRequestAsync");
            var response = await httpClient.GetAsync(requestUri);
            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                logger.LogDebug("PostCodeService end call GetHttpRequestAsync -> return string");
                return response.Content.ReadAsStringAsync().Result;
            }

            logger.LogDebug("PostCodeService end call GetHttpRequestAsync -> return Null");
            return null;
        }

        #endregion
    }
}