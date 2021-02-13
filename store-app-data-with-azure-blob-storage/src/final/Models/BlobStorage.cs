using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;


namespace FileUploader.Models
{
    public class BlobStorage : IStorage
    {
        private readonly AzureStorageConfig _storageConfig;

        public BlobStorage(IOptions<AzureStorageConfig> storageConfig)
        {
            this._storageConfig = storageConfig.Value;
        }

        public Task Initialize()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_storageConfig.ConnectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(_storageConfig.FileContainerName);
            return blobContainerClient.CreateIfNotExistsAsync();
        }

        public Task Save(Stream fileStream, string name)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_storageConfig.ConnectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(_storageConfig.FileContainerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(name);
            return blobClient.UploadAsync(fileStream);
        }

        public async Task<IEnumerable<string>> GetNames()
        {
            List<string> names = new List<string>();

            BlobServiceClient blobServiceClient = new BlobServiceClient(_storageConfig.ConnectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(_storageConfig.FileContainerName);
            
            await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync())
            {
                names.Add("\t" + blobItem.Name);
            }

            return names;
        }

        public Task<Stream> Load(string name)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_storageConfig.ConnectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(_storageConfig.FileContainerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(name);
            return blobClient.OpenReadAsync();
        }
    }
}