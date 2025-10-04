namespace ArtStore.Application.Interfaces.Services;

/// <summary>
/// Result of a file upload operation
/// </summary>
public class UploadResult
{
    public string Url { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
}

/// <summary>
/// Service for managing blob storage operations
/// </summary>
public interface IBlobStorageService
{
    /// <summary>
    /// Uploads a file to blob storage
    /// </summary>
    /// <param name="containerName">Name of the container</param>
    /// <param name="fileName">Name of the file</param>
    /// <param name="content">File content as byte array</param>
    /// <param name="contentType">MIME type of the file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Upload result with URL and dimensions</returns>
    Task<UploadResult> UploadFileAsync(string containerName, string fileName, byte[] content, string contentType, int width = 0, int height = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a file to blob storage from a stream
    /// </summary>
    /// <param name="containerName">Name of the container</param>
    /// <param name="fileName">Name of the file</param>
    /// <param name="stream">File stream</param>
    /// <param name="contentType">MIME type of the file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Upload result with URL and dimensions</returns>
    Task<UploadResult> UploadFileAsync(string containerName, string fileName, Stream stream, string contentType, int width = 0, int height = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from blob storage
    /// </summary>
    /// <param name="containerName">Name of the container</param>
    /// <param name="fileName">Name of the file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteFileAsync(string containerName, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the URL of a blob
    /// </summary>
    /// <param name="containerName">Name of the container</param>
    /// <param name="fileName">Name of the file</param>
    /// <returns>The URL of the blob</returns>
    string GetBlobUrl(string containerName, string fileName);

    /// <summary>
    /// Checks if a blob exists
    /// </summary>
    /// <param name="containerName">Name of the container</param>
    /// <param name="fileName">Name of the file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the blob exists, otherwise false</returns>
    Task<bool> BlobExistsAsync(string containerName, string fileName, CancellationToken cancellationToken = default);
}