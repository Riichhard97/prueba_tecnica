using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace Nexu.Shared.Infrastructure
{
    public sealed class AzureFileSystem : IFileSystem
    {
        private readonly BlobContainerClient _containerClient;

        public string ConnectionString { get; }
        public string ContainerName { get; }

        public AzureFileSystem(string connectionString, string containerName)
        {
            ConnectionString = connectionString;
            ContainerName = containerName;

            // Create a client that can authenticate with a connection string
            _containerClient = new BlobContainerClient(connectionString, containerName);
        }

        public Task<bool> Delete(string path, CancellationToken cancellationToken = default)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return DeleteInternal(path, cancellationToken);
        }

        private async Task<bool> DeleteInternal(string path, CancellationToken cancellationToken = default)
        {
            // Get a reference to a blob(file) by name
            var blobClient = _containerClient.GetBlobClient(path);

            var existsResponse = await blobClient.ExistsAsync(cancellationToken);
            if (!existsResponse.Value)
            {
                return false;
            }

            await blobClient.DeleteAsync(cancellationToken: cancellationToken);
            return true;
        }

        public Task<bool> Exists(string path, CancellationToken cancellationToken = default)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return ExistsInternal(path, cancellationToken);
        }

        private async Task<bool> ExistsInternal(string path, CancellationToken cancellationToken = default)
        {
            // Get a reference to a blob(file) by name
            var blobClient = _containerClient.GetBlobClient(path);

            var existsResponse = await blobClient.ExistsAsync(cancellationToken);
            return existsResponse.Value;
        }

        public Task<bool> Read(Stream outputStream, string path, CancellationToken cancellationToken = default)
        {
            if (outputStream is null)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return ReadInternal(outputStream, path, cancellationToken);
        }

        private async Task<bool> ReadInternal(Stream outputStream, string path, CancellationToken cancellationToken = default)
        {
            // Get a reference to a blob(file) by name
            var blobClient = _containerClient.GetBlobClient(path);

            var existsResponse = await blobClient.ExistsAsync(cancellationToken);
            if (!existsResponse.Value)
            {
                return false;
            }

            await blobClient.DownloadToAsync(outputStream, cancellationToken);
            return true;
        }

        public Task Save(Stream contentStream, string path,
            IDictionary<string, string> metadata = default, CancellationToken cancellationToken = default)
        {
            if (contentStream is null)
            {
                throw new ArgumentNullException(nameof(contentStream));
            }

            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return SaveInternal(contentStream, path, metadata, cancellationToken);
        }

        private async Task SaveInternal(Stream contentStream, string path,
            IDictionary<string, string> metadata = default, CancellationToken cancellationToken = default)
        {
            var blobClient = _containerClient.GetBlobClient(path);

            await blobClient.UploadAsync(contentStream, overwrite: true, cancellationToken);
            await blobClient.SetMetadataAsync(metadata, cancellationToken: cancellationToken);
        }

        public Task Copy(string source, string destination, IDictionary<string, string> metadata = default, CancellationToken cancellationToken = default)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination is null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            return CopyInternal(source, destination, metadata, cancellationToken);
        }

        private async Task CopyInternal(string source, string destination, IDictionary<string, string> metadata = default, CancellationToken cancellationToken = default)
        {
            // Get a reference to a blob(file) by name
            var sourceBlobClient = _containerClient.GetBlobClient(source);
            var destinationBlobClient = _containerClient.GetBlobClient(destination);

            await destinationBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri, cancellationToken: cancellationToken);

            if (metadata?.Count > 0)
            {
                await destinationBlobClient.SetMetadataAsync(metadata, cancellationToken: cancellationToken);
            }
            else
            {
                var sourceProperties = await sourceBlobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
                await destinationBlobClient.SetMetadataAsync(sourceProperties.Value.Metadata, cancellationToken: cancellationToken);
            }
        }

        public Task CreateDirectory(string path, CancellationToken cancellationToken = default)
        {
            // Unnecessary operation for Azure Storage service.
            // New directories are always created dynamically based on the path argument provided on Save and Copy methods.
            return Task.CompletedTask;
        }

        public async Task<DownloadResult> Download(string path, string fileName)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            await using var memoryStream = new MemoryStream();
            var blobClient = _containerClient.GetBlobClient(path);
            await blobClient.DownloadToAsync(memoryStream);

            if (memoryStream == null)
            {
                return await Task.FromResult(DownloadResult.NotFound);
            }
            var blobStream = blobClient.OpenReadAsync().Result;
            return await Task.FromResult(DownloadResult.FromStream(blobStream));
        }
    }
}
