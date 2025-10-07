using ArtStore.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;

namespace ArtStore.Infrastructure.Services.Storage;

/// <summary>
/// Local file storage implementation for development
/// </summary>
public class LocalFileStorageService : IBlobStorageService
{
    private readonly ILogger<LocalFileStorageService> _logger;
    private readonly string _storageBasePath;
    private readonly string _baseUrl;

    public LocalFileStorageService(
        IConfiguration configuration,
        ILogger<LocalFileStorageService> logger)
    {
        _logger = logger;

        _storageBasePath = configuration["LocalFileStorage:BasePath"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        _baseUrl = configuration["LocalFileStorage:BaseUrl"]
            ?? "/images";

        // Ensure directory exists
        if (!Directory.Exists(_storageBasePath))
        {
            Directory.CreateDirectory(_storageBasePath);
        }
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
            var containerPath = Path.Combine(_storageBasePath, containerName);
            if (!Directory.Exists(containerPath))
            {
                Directory.CreateDirectory(containerPath);
            }

            var filePath = Path.Combine(containerPath, fileName);

            // Create subdirectories if fileName contains a path
            var fileDirectory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(fileDirectory) && !Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

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

            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream, cancellationToken);

            _logger.LogInformation("File {FileName} uploaded successfully to {ContainerName}",
                fileName, containerName);

            return new UploadResult
            {
                Url = $"{_baseUrl}/{containerName}/{fileName}",
                Width = width,
                Height = height
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName} to {ContainerName}",
                fileName, containerName);
            throw;
        }
    }

    public Task DeleteFileAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = Path.Combine(_storageBasePath, containerName, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File {FileName} deleted from {ContainerName}",
                    fileName, containerName);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileName} from {ContainerName}",
                fileName, containerName);
            throw;
        }
    }

    public string GetBlobUrl(string containerName, string fileName)
    {
        return $"{_baseUrl}/{containerName}/{fileName}";
    }

    public Task<bool> BlobExistsAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = Path.Combine(_storageBasePath, containerName, fileName);
            return Task.FromResult(File.Exists(filePath));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if file {FileName} exists in {ContainerName}",
                fileName, containerName);
            return Task.FromResult(false);
        }
    }
}