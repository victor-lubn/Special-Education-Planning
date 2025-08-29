using Microsoft.Azure.Storage.Blob;
using System.Collections.Generic;

namespace SpecialEducationPlanning
.Api.Model.FileStorageModel
{
    public class AzureFileServiceData
    {
        public CloudBlobContainer Container { get; set; }
        public List<IListBlobItem> Blobs { get; set; }
    }
}