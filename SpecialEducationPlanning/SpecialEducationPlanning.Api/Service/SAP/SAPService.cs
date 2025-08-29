using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Flurl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.ConnectedServices.SAP;
using SpecialEducationPlanning
.Api.Model.SapConfiguration;
using SpecialEducationPlanning
.Api.Model.SapServiceModel;
using SpecialEducationPlanning
.Business.DtoModel.BuilderSapSearch;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Extensions;
using MI_1519_DV_SAPC_Request_Intl_OutClient = SpecialEducationPlanning
.Api.ConnectedServices.SAP.MI_1519_DV_SAPC_Request_Intl_OutClient;

namespace SpecialEducationPlanning
.Api.Service.Sap
{

    /// <summary>LogLogDebug
    ///     Manage Sap builders
    /// </summary>
    public class SapService : ISapService
    {

        private readonly ILogger<SapService> logger;
        private readonly IOptions<SapConfiguration> sapConfiguration;
        private readonly HttpClient httpClient;
        private const string closedSapAccountValue = "X";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="sapConfiguration"></param>
        public SapService(ILogger<SapService> logger, IOptions<SapConfiguration> sapConfiguration, HttpClient httpClient)
        {
            this.logger = logger;
            this.sapConfiguration = sapConfiguration;
            this.httpClient = httpClient;
        }

        #region Implements ISapService

        /// <summary>
        /// </summary>
        /// <param name="builderModel"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public async Task<RepositoryResponse<ValidationBuilderModel>> GetPosibleSapBuilder(BuilderModel builderModel)
        {
            logger.LogDebug("SapService: GetPosibleSapBuilder...");
            if (!String.IsNullOrEmpty(builderModel.AccountNumber))
            {
                var validationBuilder = await GetSapBuilderByAccountNumber(builderModel.AccountNumber);

                if (validationBuilder.Content.IsNotNull())
                {
                    return validationBuilder;
                }
            }

            logger.LogDebug("SapService: sapBuilder null...");
            logger.LogDebug("SapService: sapBuilder search by fields: '{tradingName}' '{postcode}' '{address1}'...", builderModel.TradingName, builderModel.Postcode, builderModel.Address1);
            var responseSapBuilderByMandatoryFields = await GetSapBuilderAsync(builderModel.TradingName, builderModel.Postcode, builderModel.Address1);

            if (responseSapBuilderByMandatoryFields.HasError())
            {
                logger.LogDebug("SapService: No GetSapBuilder found by mandatory fields");
                return new RepositoryResponse<ValidationBuilderModel>(null, ErrorCode.GenericSystemError, "responseSapBuilderByMandatoryFields");
            }

            if (responseSapBuilderByMandatoryFields.Content.IsNotNull())
            {
                logger.LogDebug("SapService: sapBuilder not null...");

                var validationBuilderModelSapByMandatoryFields = new ValidationBuilderModel();
                bool exactMatch = false;

                foreach (var sapBuilder in responseSapBuilderByMandatoryFields.Content)
                {
                    if (sapBuilder.TradingName.ToUpper().Trim().Equals(builderModel.TradingName.ToUpper().Trim()) &&
                        sapBuilder.Postcode.ToUpper().Trim().Equals(builderModel.Postcode.ToUpper().Trim()) &&
                        sapBuilder.Address1.ToUpper().Trim().Equals(builderModel.Address1.ToUpper().Trim()))
                    {
                        logger.LogDebug("SapService: Exact match found");
                        validationBuilderModelSapByMandatoryFields.Type = BuilderMatchType.Exact;
                        var item = new BuilderSapSearch() { Builder = sapBuilder, BuilderSearchType = BuilderSearchTypeEnum.Sap };
                        validationBuilderModelSapByMandatoryFields.Builders = new List<BuilderSapSearch>() { item };
                        exactMatch = true;
                        break;
                    }
                }

                if (!exactMatch)
                {
                    logger.LogDebug("SapService: Partial match found");
                    validationBuilderModelSapByMandatoryFields.Type = BuilderMatchType.NotExact;
                    validationBuilderModelSapByMandatoryFields.Builders = responseSapBuilderByMandatoryFields.Content.Select(b => new BuilderSapSearch() { Builder = b, BuilderSearchType = BuilderSearchTypeEnum.Sap }).ToList();
                }

                return new RepositoryResponse<ValidationBuilderModel>(validationBuilderModelSapByMandatoryFields);
            }

            logger.LogDebug("SapService: No matches found");
            return new RepositoryResponse<ValidationBuilderModel>();
        }


        /// <summary>
        /// Gets a possible builder from SPA by its Account Number
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public async Task<RepositoryResponse<ValidationBuilderModel>> GetPossibleSapBuilderByAccountNumber(string accountNumber)
        {
            return await GetSapBuilderByAccountNumber(accountNumber);
        }


        /// <summary>
        ///     Get sap builder by list of account numbers
        /// </summary>
        /// <returns></returns>
        async Task<RepositoryResponse<List<BuilderModel>>> ISapService.GetSapBuilder(List<string> accountNumberList, int take)
        {
            logger.LogDebug("SapService: GetSapBuilder by accountNumberList List...");
            MI_1519_DV_SAPC_Request_Intl_OutResponse result;
            var response = new RepositoryResponse<List<BuilderModel>>();

            if (accountNumberList.IsNull() || accountNumberList.Count == 0 || accountNumberList.Any(a => String.IsNullOrEmpty(a)))
            {
                logger.LogError("SapService:  GetSapBuilder by accountNumberList List Error accountNumberList == null");
                response.ErrorList.Add(ErrorCode.ArgumentErrorBusiness.GetDescription());
                return response;
            }


            logger.LogDebug("SapService:  GetSapBuilder by accountNumberList List Start connection...");
            var recon = GetSapConnection();

            try
            {
                response.Content = new List<BuilderModel>();
                foreach (string company in sapConfiguration.Value.Companies)
                {
                    logger.LogDebug($"SapService: GetSapBuilder retrieve builders by company = {company}");
                    var obj = new MI_1519_DV_SAPC_Request_Intl_OutRequest
                    {
                        MT_1519_DV_SAPC_Request_Intl = new DT_1519_DV_SAPC_Request_Intl()
                    };
                    obj.MT_1519_DV_SAPC_Request_Intl.CustRecon = new DT_1519_DV_SAPC_Request_IntlCustRecon();


                    var dt1519TdpRequestAccNumArray = accountNumberList
                        .Select(number => new DT_1519_DV_SAPC_Request_Intl_AccNum { AccNumber = number, Company = company }).ToArray();

                    obj.MT_1519_DV_SAPC_Request_Intl.CustRecon.AccNumbers = dt1519TdpRequestAccNumArray;
                    obj.MT_1519_DV_SAPC_Request_Intl.CustRecon.Max_address_return = take.ToString();
                    result = await recon.MI_1519_DV_SAPC_Request_Intl_OutAsync(obj.MT_1519_DV_SAPC_Request_Intl);

                    if (result.IsNull())
                    {
                        logger.LogError("SapService:  GetSapBuilder by accountNumberList List Error SAP Service connection");
                        response.ErrorList.Add(ErrorCode.GenericSystemError.GetDescription());
                    }

                    response.Content.AddRange(MapBuilderFromSap(result.MT_1519_DV_SAPC_Request_Intl_Resp.CustDetails));
                }
            }
            catch
            {
                recon.Abort();
            }
            finally
            {
                if (recon.State == CommunicationState.Opened)
                {
                    await recon.CloseAsync();
                }
                logger.LogDebug("SapService:  GetSapBuilder by accountNumberList List Finish connection");
            }

            return response;
        }


        /// <summary>
        ///     Get sap builder by mandatory fields
        /// </summary>
        /// <param name="tradingName"></param>
        /// <param name="postcode"></param>
        /// <param name="address"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public async Task<RepositoryResponse<List<BuilderModel>>> GetSapBuilderAsync(string tradingName, string postcode, string address)
        {
            var mandatoryFields = new List<SapByMandatoryFields> { new SapByMandatoryFields { TradingName = tradingName, Postcode = postcode, Address1 = address, Key = "1" } };

            var response = (await GetSapBuilderAsync(mandatoryFields));

            if (response.HasError())
                return new RepositoryResponse<List<BuilderModel>>(response.ErrorList);

            return new RepositoryResponse<List<BuilderModel>>(response.Content.SelectMany(d => d.Value).ToList());
        }

        public async Task<RepositoryResponse<Dictionary<string, IEnumerable<BuilderModel>>>> GetSapBuilderAsync(IEnumerable<SapByMandatoryFields> sapByMandatoryFieldsEnumerable, int take = 20)
        {
            logger.LogDebug("SapService: GetSapBuilderAsync by fields...");
            MI_1519_DV_SAPC_Request_Intl_OutResponse sapBuildersResult;

            if (sapByMandatoryFieldsEnumerable.Any(sm => string.IsNullOrEmpty(sm.TradingName) || string.IsNullOrEmpty(sm.Postcode) || string.IsNullOrEmpty(sm.Address1) || string.IsNullOrEmpty(sm.Key)))
            {
                logger.LogError("SapService: Error GetSapBuilderAsync by fields Some field == null");
                return new RepositoryResponse<Dictionary<string, IEnumerable<BuilderModel>>>(ErrorCode.ArgumentErrorBusiness.GetDescription());
            }

            logger.LogDebug("SapService: GetSapBuilderAsync by mandatory fields Start connection...");
            var recon = GetSapConnection();
            var builderDictionary = new Dictionary<string, IEnumerable<BuilderModel>>();

            try
            {
                foreach (string company in sapConfiguration.Value.Companies)
                {
                    logger.LogDebug($"SapService: GetrSapBuilderAsync retrieve builders where company = {company}");
                    var obj = new MI_1519_DV_SAPC_Request_Intl_OutRequest { MT_1519_DV_SAPC_Request_Intl = new DT_1519_DV_SAPC_Request_Intl() };


                    var requestCustReconAddresses = new List<DT_1519_DV_SAPC_Request_IntlCustReconAddresses>();
                    foreach (var sm in sapByMandatoryFieldsEnumerable)
                    {
                        requestCustReconAddresses.Add(new DT_1519_DV_SAPC_Request_IntlCustReconAddresses
                        { Address1 = sm.Address1, Postcode = sm.Postcode, Name = sm.TradingName, Record_no = sm.Key, Company = company });
                    }

                    if (take == 0)
                    {
                        take = sapConfiguration.Value.SapTake;
                    }

                    obj.MT_1519_DV_SAPC_Request_Intl.CustRecon = new DT_1519_DV_SAPC_Request_IntlCustRecon { Addresses = requestCustReconAddresses.ToArray(), Max_address_return = take.ToString() };
                    sapBuildersResult = await recon.MI_1519_DV_SAPC_Request_Intl_OutAsync(obj.MT_1519_DV_SAPC_Request_Intl);

                    if (sapBuildersResult.IsNull())
                    {
                        logger.LogError("SapService:  GetSapBuilderAsync by mandatory fields. List Error SAP Service connection");
                        return new RepositoryResponse<Dictionary<string, IEnumerable<BuilderModel>>>(null, ErrorCode.GenericSystemError, "SAP Service connection");
                    }

                    if (sapBuildersResult.MT_1519_DV_SAPC_Request_Intl_Resp.CustDetails.IsNull())
                        continue;

                    var sapGroups = sapBuildersResult.MT_1519_DV_SAPC_Request_Intl_Resp.CustDetails.GroupBy(cd => cd.Record_no);

                    foreach (var group in sapGroups)
                    {
                        builderDictionary.Add(group.Key, MapBuilderFromSap(group.ToArray()));
                    }
                }
            }
            catch
            {
                recon.Abort();
            }
            finally
            {
                logger.LogDebug("SapService: GetSapBuilderAsync by mandatory fields Finish connection");
                if (recon.State == CommunicationState.Opened)
                    await recon.CloseAsync();
            }

            return new RepositoryResponse<Dictionary<string, IEnumerable<BuilderModel>>>(builderDictionary);
        }

        #endregion

        #region Methods Public

        /// <summary>
        ///     Get sap builder by one account number
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public async Task<RepositoryResponse<List<BuilderModel>>> GetSapBuilder(string accountNumber)
        {
            return await ((ISapService)this).GetSapBuilder(new List<string> { accountNumber });
        }

        #endregion

        #region Methods Private

        /// <summary>
        ///     Return match of a builder
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private List<BuilderModel> MapBuilderFromSap(DT_1519_SAPC_DV_CustDetails_Intl[] result)
        {
            logger.LogDebug("MapBuilderFromSap started");
            var builders = new List<BuilderModel>();

            if (result.IsNotNull())
            {
                logger.LogDebug("MapBuilderFromSap has results to evaluate");

                logger.LogDebug("MapBuilderFromSap CustDetails total: {custDetailsTotal}: {json}", result.Count(), JsonConvert.SerializeObject(result));
                foreach (var item in result.Where(cd => !cd.AccountNumber.IsNullOrEmpty() && cd.BuilderStatus.ToUpper() != closedSapAccountValue))
                {
                    try
                    {
                        logger.LogDebug("Data received from SAP: {jsonData}", JsonConvert.SerializeObject(item));
                        var builder = new BuilderModel
                        {
                            Email = item.Email,
                            AccountNumber = item.AccountNumber,
                            Address1 = item.Address1,
                            Address2 = item.Address2,
                            Address3 = item.Address3,
                            Name = item.Name,
                            MobileNumber = item.MobileNumber.NormaliseNumber(),
                            OwningAiepCode = item.AiepID,
                            OwningAiepName = item.AiepName,
                            Postcode = item.Postcode.NormalisePostcode(),
                            TradingName = item.TradingName,
                            LandLineNumber = item.LandLineNumber.NormaliseNumber(),
                            BuilderStatus = item.BuilderStatus.Equals(closedSapAccountValue) ? BuilderStatus.Closed :BuilderStatus.Active
                        };
                        logger.LogWarning("AiepCode and AiepName being ingested from SAP but SAPAccountStatus not. This is an incomplete solution.");
                        logger.LogDebug("MapBuilderFromSap matched account: {account}", builder.AccountNumber);
                        builders.Add(builder);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error parsing SAP data");
                    }
                }
            }
            return builders;
        }



        public async Task<string> HealthCheck()
        {
            logger.LogDebug("SapService called HealthCheck");
            string output;
            try
            {
                var uri = new Url(sapConfiguration.Value.SapConnectionCredentials.Url).ToUri();
                logger.LogDebug("SapService HealthCheck URL: {URL}", sapConfiguration.Value.SapConnectionCredentials.Url);
                var response = (await this.httpClient.GetAsync(uri));
                if (response.IsSuccessStatusCode)
                {
                    output = null;
                }
                else
                {
                    output = string.Format("SapService HealthCheck:{0} Content -> {1}", response.StatusCode.ToString(), response.Content.ToString());   
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                output = string.Format("SapService HealthCheck:{0} Content -> {1}", ex);
            }

            return output;
        }


        /// <summary>
        ///     Create the SAP connection and match the builder
        /// </summary>
        /// <returns></returns>
        private MI_1519_DV_SAPC_Request_Intl_OutClient GetSapConnection()
        {
            logger.LogDebug("SapService: GetSapConnection...");
            var basicAuthEndpoint = new EndpointAddress(sapConfiguration.Value.SapConnectionCredentials.Url);
            var recon = new MI_1519_DV_SAPC_Request_Intl_OutClient(MI_1519_DV_SAPC_Request_Intl_OutClient.EndpointConfiguration.HTTPS_Port, basicAuthEndpoint);

            ((BasicHttpBinding)recon.Endpoint.Binding).Security.Transport.ClientCredentialType =
                HttpClientCredentialType.Basic;

            recon.ClientCredentials.UserName.UserName = sapConfiguration.Value.SapConnectionCredentials.User;
            recon.ClientCredentials.UserName.Password = sapConfiguration.Value.SapConnectionCredentials.Password;

            recon.ClientCredentials.ServiceCertificate.SslCertificateAuthentication =
                new X509ServiceCertificateAuthentication
                {
                    CertificateValidationMode =
                X509CertificateValidationMode.None
                };

            logger.LogDebug("SapService: GetSapConnection finish");

            return recon;
        }

        private async Task<RepositoryResponse<ValidationBuilderModel>> GetSapBuilderByAccountNumber(string accountNumber)
        {
            logger.LogDebug("SapService: GetPosibleSapBuilder GetSapBuilder...");
            var responseSapBuildersByAccountNumber = await GetSapBuilder(accountNumber);

            if (!responseSapBuildersByAccountNumber.HasError() && responseSapBuildersByAccountNumber.Content.IsNotNull() && responseSapBuildersByAccountNumber.Content.Any())
            {
                logger.LogDebug("SapService: sapBuilder not null...");
                var validationBuilderModelSapByAccountNumber = new ValidationBuilderModel
                {
                    Type = BuilderMatchType.Exact,
                    Builders = responseSapBuildersByAccountNumber.Content.Select(b => new BuilderSapSearch() { Builder = b, BuilderSearchType = BuilderSearchTypeEnum.Sap }).ToList()
                };

                return new RepositoryResponse<ValidationBuilderModel>(validationBuilderModelSapByAccountNumber);
            }

            logger.LogDebug("SapService: No matches found");
            return new RepositoryResponse<ValidationBuilderModel>();
        }

        #endregion

    }

}
