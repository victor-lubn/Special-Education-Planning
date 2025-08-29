using Koa.IO.FileSystem.AzureStorage;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Model.FileStorageModel;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.IService.IFileStorage;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Model.FileStorageModel;

namespace SpecialEducationPlanning
.Api.Service.FileStorage
{

    /// <summary>
    ///     File storage service
    /// </summary>
    public class FileStorageService<T> : IFileStorageService<T> where T : IAzureOptionsConfiguration
    {

        private readonly AzureFileSystem azureFs;
        private readonly ILogger<IFileStorageService<T>> logger;
        private readonly CloudBlobClient client;

        /// <summary>
        ///     Createas a new instance of <see cref="FileStorageService" />
        /// </summary>
        /// <param name="azureStorageConfiguration">Azure Storage Configuration</param>
        /// <param name="logger">Class logger</param>
        public FileStorageService(IConfiguration configuration,
            ILogger<IFileStorageService<T>> logger)
        {
            this.logger = logger;
            this.logger.LogDebug("Creating {type}...", GetType());
            this.logger.LogDebug("Creating credentials...", GetType());

            var azure = ReturnAzureStorageConfiguration(configuration);
            var azureAccountName = azure.AzureAccountName;
            var azureKeyValue = azure.AzureKeyValue;
            var credentials = new StorageCredentials(azureAccountName, azureKeyValue);

            this.logger.LogDebug("Creating cloud storage...", GetType());

            var cloudStorageAccount = new CloudStorageAccount(credentials, true);

            this.logger.LogDebug("Creating Azure File System...", GetType());

            azureFs = new AzureFileSystem(cloudStorageAccount);

            this.logger.LogDebug("{type} created...", GetType());
            this.client = cloudStorageAccount.CreateCloudBlobClient();
        }

        #region Methods IFileStorageService

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(string path)
        {
            logger.LogDebug("FileStorageService called DeleteAsync");

            logger.LogDebug("Deleting file '{path}'", path);

            if (!await ExistsAsync(path))
            {
                return false;
            }

            var deleted = await azureFs.DeleteAsync(path);
            logger.LogDebug("File '{path} deleted: {deleted}'", path, deleted);

            logger.LogDebug("FileStorageService end call DeleteAsync -> return Bool");

            return deleted;
        }

        /// <inheritdoc />
        public async Task<Stream> DownloadAsync(string path)
        {
            logger.LogDebug("FileStorageService called DownloadAsync");

#if DEBUG

            // HACK: KOA is using Path.DirectorySeparatorChar to split the path to get the Container name (the first chunk)
            // In Azure we have Linux, where DirectorySeparatorChar is '/'. When running in Windows, we need to use '\\'
            // Only to run this in DEBUG. Production should be fine unless we change to Windows
            path = path.Replace('/', Path.DirectorySeparatorChar);
#endif
            logger.LogDebug("Downloading file '{path}'", path);

            if (!await ExistsAsync(path))
            {
                logger.LogWarning("File '{path}' does not exist", path);

                return null;
            }

            logger.LogDebug("FileStorageService end call DownloadAsync -> return Stream");

            return await azureFs.OpenAsync(path);
        }

        /// <inheritdoc />
        public async Task<string> OverrideAsync(Stream file, string path)
        {
            logger.LogDebug("FileStorageService called OverrideAsync");

            if (!await ExistsAsync(path))
            {
                logger.LogWarning("File '{path}' does not exist", path);

                return null;
            }

            logger.LogDebug("Uploading file to '{path}'", path);
            await azureFs.SaveAsync(path, file);

            logger.LogDebug("FileStorageService end call OverrideAsync -> return String");

            return path;
        }

        /// <inheritdoc />
        public async Task<string> UploadAsync<U>(Stream file)
        {
            logger.LogDebug("FileStorageService called UploadAsync");

            if (file.IsNull())
            {
                logger.LogDebug("FileStorageService end call UploadAsync -> exception");

                throw new ArgumentNullException("file");
            }

            var path = CreatePath<U>();
            logger.LogDebug("Uploading file to '{path}'", path);
            await azureFs.SaveAsync(path, file);

            logger.LogDebug("FileStorageService end call UploadAsync -> return String");

            return path;
        }

        public async Task<string> UploadCreatioAsync(Stream file, string projectCode, string planCode)
        {
            logger.LogDebug("FileStorageService called UploadCreatioAsync");

            if (file.IsNull())
            {
                logger.LogDebug("FileStorageService end call UploadCreatioAsync -> exception");

                throw new ArgumentNullException("file");
            }

            var path = CreateCreatioPath(projectCode, planCode);

            logger.LogDebug("Uploading file to '{path}'", path);

            await azureFs.SaveAsync(path, file);

            logger.LogDebug("FileStorageService end call UploadCreatioAsync -> return String");

            return path;
        }

        private string CreateCreatioPath(string projectCode, string planCode)
        {
            logger.LogDebug("FileStorageService called CreateCreatioPath");

            var date = DateTime.UtcNow;
            var epochTime = EpochTime.GetIntDate(date);

            logger.LogDebug("FileStorageService end call CreateCreatioPath -> return String");
            var path = $"{projectCode}/{planCode}_PlanPreview_{epochTime}";

            return path;
        }

        /// <inheritdoc />
        public async Task<bool> UploadOverrideAsync(Stream file, string path)
        {
            logger.LogDebug("FileStorageService called UploadOverrideAsync");

            if (file.IsNull())
            {
                throw new ArgumentNullException("file");
            }

            logger.LogDebug("Uploading file to '{path}'", path);
            await azureFs.SaveAsync(path, file);

            logger.LogDebug("FileStorageService end call UploadOverrideAsync -> return Bool");

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> UploadOverrideVersionAsync(Stream file, VersionModel versionModel)
        {
            logger.LogDebug("FileStorageService called UploadOverrideVersionAsync");

            logger.LogDebug("FileStorageService end call UploadOverrideVersionAsync -> return Bool Call CreateVersionPath");

            return await UploadOverrideAsync(file, versionModel.RomPath.IsNullOrEmpty() ? CreateVersionPath(versionModel) : versionModel.RomPath);
        }

        #endregion

        #region Methods Private

        /// <inheritdoc />
        private string CreatePath<U>()
        {
            logger.LogDebug("FileStorageService called CreatePath");

            var date = DateTime.UtcNow;
            var uniqueId = Guid.NewGuid();

            logger.LogDebug("FileStorageService end call CreatePath -> return String");

            return Path.Combine(typeof(U).Name, date.Year.ToString("0000"), date.Month.ToString("00"),
                uniqueId.ToUserString());
        }

        private string CreateVersionPath(VersionModel versionModel)
        {
            logger.LogDebug("FileStorageService called CreateVersionPath");

            var date = DateTime.UtcNow;
            var uniqueId = Guid.NewGuid();

            if (versionModel.Plan.IsNull() || versionModel.ExternalCode.IsNullOrEmpty() ||
                versionModel.Plan.PlanCode.IsNullOrEmpty())
            {
                logger.LogDebug("FileStorageService end call CreateVersionPath -> exception");

                throw new ArgumentNullException("CreateVerionPath: versionModel null property");
            }

            var path = Path.Combine(versionModel.GetType().Name, date.Year.ToString("0000"), date.Month.ToString("00"),
                versionModel.Plan.PlanCode,
                $"{uniqueId.ToUserString()}_v{versionModel.VersionNumber}");

            versionModel.RomPath = path;

            logger.LogDebug("FileStorageService end call CreateVersionPath -> return String");

            return path;
        }

        private async Task<bool> ExistsAsync(string path)
        {
            logger.LogDebug("FileStorageService called ExistsAsync");

            logger.LogDebug("Checking if file '{path}' exists", path);
            var exists = !path.IsNullOrEmpty() && await azureFs.ExistsAsync(path);
            logger.LogDebug("'{path}' exists: {exists}", path, exists);

            logger.LogDebug("FileStorageService end call ExistsAsync -> return Bool");

            return exists;
        }

        private IAzureOptionsConfiguration ReturnAzureStorageConfiguration(IConfiguration configuration)
        {
            var azure = Activator.CreateInstance<T>();
            configuration.GetSection(typeof(T).Name[..^"Configuration".Length]).Bind(azure);
            return azure;
        }

        #endregion

    }

}