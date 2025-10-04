using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ArtStore.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace ArtStore.Infrastructure.Services.Storage;

/// <summary>
/// Azure Blob Storage implementation
/// </summary>
public class AzureBlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<AzureBlobStorageService> _logger;
    private readonly string _baseUrl;

    public AzureBlobStorageService(
        IConfiguration configuration,
        ILogger<AzureBlobStorageService> logger)
    {
        _logger = logger;

        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Azure Blob Storage connection string is not configured");
        }

        _blobServiceClient = new BlobServiceClient(connectionString);
        _baseUrl = configuration["AzureBlobStorage:BaseUrl"] ?? _blobServiceClient.Uri.ToString();
    }

    public async Task<UploadResult> UploadFileAsync(
        string containerName,
        string fileName,
        byte[] content,
        string contentType,
        int width = 0,
        int height = 0,
        CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(content);
        return await UploadFileAsync(containerName, fileName, stream, contentType, width, height, cancellationToken);
    }

    public async Task<UploadResult> UploadFileAsync(
        string containerName,
        string fileName,
        Stream stream,
        string contentType,
        int width = 0,
        int height = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            // Create container if it doesn't exist
            await containerClient.CreateIfNotExistsAsync(
                PublicAccessType.Blob,
                cancellationToken: cancellationToken);

            // Get image dimensions if it's an image
            if ((width == 0 || height == 0) && contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    stream.Position = 0;
                    using var image = await Image.LoadAsync(stream, cancellationToken);
                    width = image.Width;
                    height = image.Height;
                    stream.Position = 0;
                }
                catch
                {
                    // If image processing fails, continue without dimensions
                }
            }

            var blobClient = containerClient.GetBlobClient(fileName);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            await blobClient.UploadAsync(
                stream,
                new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                },
                cancellationToken);

            _logger.LogInformation("File {FileName} uploaded successfully to container {ContainerName}",
                fileName, containerName);

            return new UploadResult
            {
                Url = blobClient.Uri.ToString(),
                Width = width,
                Height = height
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName} to container {ContainerName}",
                fileName, containerName);
            throw;
        }
    }

    public async Task DeleteFileAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

            _logger.LogInformation("File {FileName} deleted from container {ContainerName}",
                fileName, containerName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileName} from container {ContainerName}",
                fileName, containerName);
            throw;
        }
    }

    public string GetBlobUrl(string containerName, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);
        return blobClient.Uri.ToString();
    }

    public async Task<bool> BlobExistsAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            return await blobClient.ExistsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if blob {FileName} exists in container {ContainerName}",
                fileName, containerName);
            return false;
        }
    }
}
