using SpecialEducationPlanning
.Business.IService.IFileStorage;

namespace SpecialEducationPlanning
.Business.Model.FileStorageModel.Base
{
    /// <summary>
    /// </summary>
    public abstract class AzureStorageConfigurationBase : IAzureOptionsConfiguration
    {
        /// <summary>
        /// </summary>
        public string AzureAccountName { get; set; }

        /// <summary>
        /// </summary>
        public string AzureKeyValue { get; set; }
    }
}