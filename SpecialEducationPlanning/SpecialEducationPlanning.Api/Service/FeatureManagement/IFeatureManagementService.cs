using System.Threading.Tasks;

namespace SpecialEducationPlanning
.Api.Service.FeatureManagement
{
    public interface IFeatureManagementService
    {
        /// <summary>
        /// Get boolean Feature Flag value via Flag Name
        /// </summary>
        /// <param name="flagname"></param>
        /// <param name="claimsIdentity"></param>
        /// <returns>Boolean indicating if the feature flag is turned on or off.</returns>
        Task<bool> GetFeatureFlagAsync(string flagname, System.Security.Claims.ClaimsIdentity claimsIdentity);

        /// <summary>
        /// Get boolean Feature Flag value via Flag Name
        /// </summary>
        /// <param name="flagname"></param>
        /// <param name="claimsIdentity"></param>
        /// <returns>Boolean indicating if the feature flag is turned on or off.</returns>
        Task<FeatureFlagEnum> GetFeatureStringFlagAsync(string flagname, System.Security.Claims.ClaimsIdentity claimsIdentity);

     
        /// <summary>
        /// Health Check for Launch Darkly
        /// </summary>
        /// <param name="flagname"></param>
        /// <param name="claimsIdentity"></param>
        /// <returns>Returns bool indicating if the Health Check was successful</returns>
        Task<bool> HealthCheck();
    }
}
