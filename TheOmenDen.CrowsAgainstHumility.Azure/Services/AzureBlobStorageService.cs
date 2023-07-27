using Azure.ResourceManager.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Services;
internal class AzureBlobStorageService : IAzureBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureBlobStorageService(BlobServiceClient blobServiceClient) => _blobServiceClient = blobServiceClient;

    public async Task<bool> DoesBlobExistAsync(string fileName, BlobContainerResource container,
        CancellationToken cancellationToken = default)
    {
        var containerClient = await GetContainerClientAsync(container, cancellationToken);
        var blobClient = containerClient.GetBlobClient(fileName);
        return await blobClient.ExistsAsync(cancellationToken);
    }

    public async Task UploadFileToBlobAsync(byte[] content, string fileName, BlobContainerResource container, CancellationToken cancellationToken = default)
    {
        await using var stream = new MemoryStream(content);
        var containerClient = await GetContainerClientAsync(container, cancellationToken);
        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(stream, cancellationToken);
    }

    public async Task<byte[]> GetFileFromBlobAsync(string fileName, BlobContainerResource container, CancellationToken cancellationToken = default)
    {
        var containerClient = await GetContainerClientAsync(container, cancellationToken);
        var blobClient = containerClient.GetBlobClient(fileName);

        if (!await blobClient.ExistsAsync(cancellationToken))
        {
            return Array.Empty<byte>();
        }

        var response = await blobClient.DownloadAsync(cancellationToken);

        if (!response.HasValue)
        {
            return Array.Empty<byte>();
        }

        await using var stream = new MemoryStream();
        await response.Value.Content.CopyToAsync(stream, cancellationToken);

        return stream.ToArray();
    }

    public async Task DeleteFileFromBlob(string fileName, BlobContainerResource container, CancellationToken cancellationToken = default)
    {
        var containerClient = await GetContainerClientAsync(container, cancellationToken);
        var blobClient = containerClient.GetBlobClient(fileName);

        if (await blobClient.ExistsAsync(cancellationToken))
        {
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
        }
    }

    private async Task<BlobContainerClient> GetContainerClientAsync(BlobContainerResource container,
        CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container.ToString());
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);
        return containerClient;
    }
}
