using System.IO;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.IService.IFileStorage;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Model.FileStorageModel;

namespace SpecialEducationPlanning
.Business.IService
{

    /// <summary>
    ///     File service client for Azure
    /// </summary>
    public interface IFileStorageService<T> where T : IAzureOptionsConfiguration
    {

        #region Methods Public

        /// <summary>
        ///     File deletion from remote storage
        /// </summary>
        /// <param name="path">Remote path (i.e. blobcontainer\\file.txt)</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(string path);

        /// <summary>
        ///     File download
        /// </summary>
        /// <param name="path">Remote path (i.e. blobcontainer\\file.txt)</param>
        /// <returns></returns>
        Task<Stream> DownloadAsync(string path);

        /// <summary>
        ///     Given a type T, based on the path and overrides the file to Azure Blob
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<string> OverrideAsync(Stream file, string path);

        /// <summary>
        ///     Uploads a new file associated to a Type
        /// </summary>
        /// <typeparam name="T">Type associated with the file</typeparam>
        /// <param name="file">File stream</param>
        /// <returns>Path where the file has been uploaded to</returns>
        Task<string> UploadAsync<T>(Stream file);

        /// <summary>
        ///     Uploads or override a new file given a path
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<bool> UploadOverrideAsync(Stream file, string path);

        /// <summary>
        ///     Uploads or override a new file given a VersionModel
        /// </summary>
        /// <param name="file"></param>
        /// <param name="versionModel"></param>
        /// <returns></returns>
        Task<bool> UploadOverrideVersionAsync(Stream file, VersionModel versionModel);

        Task<string> UploadCreatioAsync(Stream file, string projectCode, string planCode);

        #endregion

    }

}