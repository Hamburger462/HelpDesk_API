namespace HelpDesk.Services.Storage
{
    public interface IFileStorage
    {
        Task UploadAsync(string path, Stream content, string contentType, CancellationToken cancellationToken = default);

        // Must be a no-op for an empty list. Missing files should not throw.
        Task DeleteAsync(IEnumerable<string> paths, CancellationToken cancellationToken = default);

        // Returns null when the file does not exist. The caller disposes the stream.
        Task<Stream?> OpenReadAsync(string path, CancellationToken cancellationToken = default);
    }
}