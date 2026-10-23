namespace HelpDesk.Services.Storage
{
    // Stores files on the server's disk, outside wwwroot so nothing is served without going through the API.
    // Root folder: configuration key "Storage:RootPath" (relative to the app root, or absolute).
    public class LocalFileStorage : IFileStorage
    {
        private readonly string _root;

        public LocalFileStorage(IWebHostEnvironment env, IConfiguration config)
        {
            var configured = config["Storage:RootPath"] ?? "App_Data/uploads";
            _root = Path.GetFullPath(Path.Combine(env.ContentRootPath, configured));
            Directory.CreateDirectory(_root);
        }

        public async Task UploadAsync(string path, Stream content, string contentType, CancellationToken cancellationToken = default)
        {
            var fullPath = Resolve(path);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            try
            {
                await using var file = new FileStream(
                    fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
                await content.CopyToAsync(file, cancellationToken);
            }
            catch
            {
                if (File.Exists(fullPath)) File.Delete(fullPath);   // don't leave a half-written file
                throw;
            }
        }

        public Task DeleteAsync(IEnumerable<string> paths, CancellationToken cancellationToken = default)
        {
            foreach (var path in paths)
            {
                var fullPath = Resolve(path);
                File.Delete(fullPath);   // does nothing if the file is already gone

                // Tidy up the per-ticket folder once it is empty.
                var directory = Path.GetDirectoryName(fullPath)!;
                try
                {
                    if (directory != _root && Directory.Exists(directory)
                        && !Directory.EnumerateFileSystemEntries(directory).Any())
                        Directory.Delete(directory);
                }
                catch (IOException) { /* another request just added a file; leave the folder */ }
            }

            return Task.CompletedTask;
        }

        public Task<Stream?> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = Resolve(path);

            Stream? stream = File.Exists(fullPath)
                ? new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920,
                    FileOptions.Asynchronous | FileOptions.SequentialScan)
                : null;

            return Task.FromResult(stream);
        }

        // Guards against path traversal: the result must stay inside the root folder.
        private string Resolve(string path)
        {
            var fullPath = Path.GetFullPath(Path.Combine(_root, path));
            if (!fullPath.StartsWith(_root + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                throw new InvalidOperationException("Invalid storage path.");
            return fullPath;
        }
    }
}