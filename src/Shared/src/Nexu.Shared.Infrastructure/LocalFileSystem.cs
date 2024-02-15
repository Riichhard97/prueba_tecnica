using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nexu.Shared.Infrastructure
{
    public sealed class LocalFileSystem : IFileSystem
    {
        public string Directory { get; }

        public LocalFileSystem(string directory)
        {
            if (!System.IO.Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException($"Base local file directory {directory} does not exist.");
            }

            Directory = directory;
        }

        public Task<bool> Delete(string path, CancellationToken cancellationToken = default)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var fileName = Path.Combine(Directory, path);
            var fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> Exists(string path, CancellationToken cancellationToken = default)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var fileName = Path.Combine(Directory, path);
            var exists = File.Exists(fileName);
            return Task.FromResult(exists);
        }

        public async Task<bool> Read(Stream outputStream, string path, CancellationToken cancellationToken = default)
        {
            if (outputStream is null)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var fileName = Path.Combine(Directory, path);
            if (File.Exists(fileName))
            {
                using (var sourceStream = File.Open(fileName, FileMode.Open))
                {
                    await sourceStream.CopyToAsync(outputStream, cancellationToken).ConfigureAwait(false);
                }

                return true;
            }

            return false;
        }

        public async Task Save(Stream contentStream, string path,
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

            var fullPath = Path.Combine(Directory, path);
            var directory = Path.GetDirectoryName(fullPath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
                //throw new DirectoryNotFoundException($"File directory {directory} does not exist.");
            }

            using (var destinationStream = File.Create(fullPath))
            {
                await contentStream.CopyToAsync(destinationStream, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<DownloadResult> Download(string path, string fileName)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var stream = new MemoryStream();
            try
            {
                if (await Read(stream, path))
                {
                    stream.Position = 0;
                    return DownloadResult.FromStream(stream);
                }
            }
            catch (FileNotFoundException)
            {
                stream.Dispose();
            }

            return DownloadResult.NotFound;
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

            var sourceFileName = Path.Combine(Directory, source);
            if (File.Exists(sourceFileName))
            {
                var targetFileName = Path.Combine(Directory, destination);
                var targetDirectory = Path.GetDirectoryName(targetFileName);
                if (!System.IO.Directory.Exists(targetDirectory))
                {
                    System.IO.Directory.CreateDirectory(targetDirectory);
                }

                File.Copy(sourceFileName, targetFileName, true);
            }

            return Task.CompletedTask;
        }

        public Task CreateDirectory(string path, CancellationToken cancellationToken = default)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var directoryPath = Path.Combine(Directory, path);
            var directory = new DirectoryInfo(directoryPath);
            if (!directory.Exists)
            {
                directory.Create();
            }

            return Task.CompletedTask;
        }
    }
}
