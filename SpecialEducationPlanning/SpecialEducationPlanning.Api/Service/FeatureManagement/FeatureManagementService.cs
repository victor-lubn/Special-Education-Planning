using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using SpecialEducationPlanning
.Api.Configuration.FeatureManagement;
using SpecialEducationPlanning
.Business.Repository;
using LaunchDarkly.Sdk.Server.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using Microsoft.Extensions.Options;
using System.Text.Json;
using SpecialEducationPlanning
.Api.Model.FeatureManagementModel;
using SpecialEducationPlanning
.Business.Model;

namespace SpecialEducationPlanning
.Api.Service.FeatureManagement
{
    public class FeatureManagementService : IFeatureManagementService
    {
        private readonly FeatureManagementCountryConfiguration _countryConfiguration;
        private readonly LaunchDarklyConfiguration _launchDarklyConfiguration;
        private readonly ILogger<FeatureManagementService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IAiepRepository _AiepRepository;
        private readonly ILdClient _ldClient;
        private readonly HttpClient _httpClient;

        private const string AiepCustomField = "Aiep";
        private const string ContentTypeSuffix = "domain-model=launchdarkly.semanticpatch";

        public FeatureManagementService(
            FeatureManagementCountryConfiguration countryConfiguration,
            IOptions<LaunchDarklyConfiguration>  launchDarklyConfiguration,
            ILogger<FeatureManagementService> logger,
            IUserRepository userRepository,
            IAiepRepository AiepRepository,
            ILdClient ldClient,
            HttpClient httpClient)
        {
            this._countryConfiguration = countryConfiguration;
            this._launchDarklyConfiguration = launchDarklyConfiguration.Value;
            this._userRepository = userRepository;
            this._AiepRepository = AiepRepository;
            this._logger = logger;
            this._ldClient = ldClient;
            this._httpClient = httpClient;

            this._httpClient.BaseAddress = new Uri(this._launchDarklyConfiguration.HealthCheck.Api_BaseUrl);
            this._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(this._launchDarklyConfiguration.HealthCheck.Access_Token);
        }

        /// <inheritdoc/>
        public async Task<bool> GetFeatureFlagAsync(string flagName, ClaimsIdentity claimsIdentity)
        {
            this._logger.LogDebug("FeatureManagementService called GetFeatureFlag for feature-flag {flagName} and user {email}", flagName, claimsIdentity.Name);

            if (await this.GetFeatureFlagInternalAsync(flagName, claimsIdentity))
            {
                this._logger.LogDebug($"GetFeatureFlagAsync -> Flag {flagName} is ON");

                return true;
            }
            else
            {
                this._logger.LogDebug($"GetFeatureFlagAsync -> Flag {flagName} is OFF");

                return false;
            }
        }

        public async Task<FeatureFlagEnum> GetFeatureStringFlagAsync(string flagName, ClaimsIdentity claimsIdentity)
        {
            this._logger.LogDebug("FeatureManagementService called GetFeatureFlagStringAsync for feature-flag {flagName} and user {email}", flagName, claimsIdentity.Name);

            if (!this.CheckParameters(flagName, claimsIdentity)) return FeatureFlagEnum.Off;

            string email = claimsIdentity.Name;

            string AiepCode = await this.GetAiepCodeByUserEmail(email);

            if (string.IsNullOrWhiteSpace(AiepCode))
            {
                this._logger.LogWarning("Invalid Aiep code: Null or Empty");
                return FeatureFlagEnum.Off;
            }

            this._logger.LogDebug("Attempting to create LaunchDarkly User with {flagName} and user {email}", flagName, email);

            LaunchDarkly.Sdk.Context context = CreateLdarklyContext(email, AiepCode);

            string flagValue = this._ldClient.StringVariation(flagName, context, Enum.GetName(typeof(FeatureFlagEnum), FeatureFlagEnum.Off));

            this._logger.LogDebug("FeatureManagementService end call GetFeatureFlag -> return {flagValue}", flagValue);

            bool validState = Enum.TryParse<FeatureFlagEnum>(flagValue, out FeatureFlagEnum flag);

            return validState ? flag : FeatureFlagEnum.Off;
        }


        /// <inheritdoc/>
        public async Task<bool> HealthCheck()
        {
            this._logger.LogDebug("FeatureManagementService called HealthCheck");

            string healthCheckFlag = this._launchDarklyConfiguration.HealthCheck.Flag_Name;
            string email = this._launchDarklyConfiguration.HealthCheck.User_Email;

            //Create new claim with email passed in as this is an anonymous call.
            ClaimsIdentity claimsIdentity = new(new Claim[] { new Claim(ClaimTypes.Name, email) });

            this._logger.LogDebug("Getting {flagName} current value", healthCheckFlag);

            FeatureFlagEnum currentFlagState = await this.GetFeatureStringFlagAsync(healthCheckFlag, claimsIdentity);

            if (Enum.GetName(typeof(FeatureFlagEnum), FeatureFlagEnum.On) == currentFlagState.ToString())
            {
                return true;
            }
            else
            {
                this._logger.LogWarning("Returned {flagName} : False", healthCheckFlag);
                return false;
            }
        }

        #region Privates

        private async Task<bool> GetFeatureFlagInternalAsync(string flagName, ClaimsIdentity claimsIdentity)
        {
            if (!this.CheckParameters(flagName, claimsIdentity)) return false;

            string email = claimsIdentity.Name;

            string AiepCode = await this.GetAiepCodeByUserEmail(email);

            if (string.IsNullOrWhiteSpace(AiepCode))
            {
                this._logger.LogWarning("Invalid Aiep code: Null or Empty");
                return false;
            }

            this._logger.LogDebug("Attempting to create LaunchDarkly User with {flagName} and user {email}", flagName, email);

            LaunchDarkly.Sdk.Context context = CreateLdarklyContext(email, AiepCode);

            bool flagValue = this._ldClient.BoolVariation(flagName, context, false);
            this._logger.LogDebug("FeatureManagementService end call GetFeatureFlag -> return {flagValue}", flagValue);
            return flagValue;
        }

        /// <summary>
        /// Check Parameters passed in for valid values.
        /// If they arent valid, return false to be on the safe side.
        /// </summary>
        /// <param name="flagName"></param>
        /// <param name="claimsIdentity"></param>
        /// <returns></returns>
        private bool CheckParameters(string flagName, ClaimsIdentity claimsIdentity)
        {
            bool connected = this._ldClient.DataSourceStatusProvider.WaitForAsync(DataSourceState.Valid, TimeSpan.FromSeconds(10)).Result;
            if (!connected)
            {
                this._logger.LogWarning("Failed to connect LdClient to valid state");
                return false;
            }

            if (!this._ldClient.Initialized)
            {
                this._logger.LogWarning("LdClient is not initialized");
                return false;
            }

            if (string.IsNullOrWhiteSpace(claimsIdentity.Name))
            {
                this._logger.LogWarning("Invalid Email: Null or empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(flagName))
            {
                this._logger.LogWarning("Invalid Flag for Launch Darkly: Null or empty");
                return false;
            }


            return true;
        }

        /// <summary>
        /// Get Aiep code by User Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Aiep code</returns>
        private async Task<string> GetAiepCodeByUserEmail(string email)
        {
            string AiepCode = string.Empty;
            RepositoryResponse<UserModel> userRepositoryResponse = await this._userRepository.GetUserByEmailAsync(email);

            if (userRepositoryResponse.Content is null)
            {
                this._logger.LogWarning("Unable to retrieve user with email: {email}", email);
                return AiepCode;
            }
            
            int AiepId = userRepositoryResponse.Content.AiepId.Value;

            RepositoryResponse<AiepModel> AiepRepositoryResponse = await this._AiepRepository.GetAiepByIdIgnoreAclAsync(AiepId);

            if (AiepRepositoryResponse.Content is null)
            {
                this._logger.LogWarning("Unable to retrieve Aiep with id: {AiepId}", AiepId);
                return AiepCode;
            }

            return AiepRepositoryResponse.Content.AiepCode;
        }

        /// <summary>
        /// Set up and build an instance of the Launch Darkly Context
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="AiepCode"></param>
        /// <returns>Launch Darkly Context</returns>
        private LaunchDarkly.Sdk.Context CreateLdarklyContext(string userEmail, string AiepCode)
        {
             LaunchDarkly.Sdk.Context LDContext = LaunchDarkly.Sdk.Context.Builder(userEmail)
             .Set("Email", userEmail)
             .Set("Country", this._countryConfiguration.Country)
             .Set(FeatureManagementService.AiepCustomField, AiepCode)
             .Build();

            return LDContext;
        }

        /// <summary>
        /// Build up string content for the Http call to Launch Darkly's API
        /// </summary>
        /// <param name="on"></param>
        /// <returns>String content ready for a Http Call.</returns>
        /// https://apidocs.launchdarkly.com/tag/Feature-flags#operation/patchFeatureFlag
        private StringContent BuildUpHttpContent(bool state)
        {
            string kind = state ? "turnFlagOn" : "turnFlagOff";
            LaunchDarklyUpdateRequestModel contentObject = new()
            {
                environmentKey = this._launchDarklyConfiguration.HealthCheck.Envionment_Key,
                instructions = new()
                {
                    new Instruction()
                    {
                        kind = kind,
                    }
                }
            };

            string contentJson = JsonSerializer.Serialize(contentObject);
            StringContent content = new(contentJson);

            //.NET auto validates the content type.
            // Any custom content type will be declared invalid.
            // To combat this we need to remove the existing header and add the new custom one without validation.
            content.Headers.Remove("Content-Type");
            content.Headers.TryAddWithoutValidation("Content-Type", $"application/json; {ContentTypeSuffix}");
            return content;
        }

        #endregion
    }
}
