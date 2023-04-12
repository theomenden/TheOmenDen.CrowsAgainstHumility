using Azure.ResourceManager.Storage;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Storage.Services;
public interface IAzureBlobStorageService
{
    Task<bool> DoesBlobExistAsync(string fileName, BlobContainerResource container, CancellationToken cancellationToken = default);
    Task UploadFileToBlobAsync(byte[] content, string fileName, BlobContainerResource container, CancellationToken cancellationToken = default);
    Task<byte[]> GetFileFromBlobAsync(string fileName, BlobContainerResource container, CancellationToken cancellationToken = default);
    Task DeleteFileFromBlob(string fileName, BlobContainerResource container, CancellationToken cancellationToken = default);
}
