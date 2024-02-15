using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Nexu.Shared.Infrastructure.Legacy;

namespace Nexu.Shared.Infrastructure
{
    public interface IFileSystem
    {
        Task<bool> Delete(string path, CancellationToken cancellationToken = default);

        Task<bool> Exists(string path, CancellationToken cancellationToken = default);

        Task<bool> Read(Stream outputStream, string path, CancellationToken cancellationToken = default);

        //Task<string> Save(Stream contentStream, string container, string extension, string fileName = null, IDictionary<string, string> metadata = null);
        Task CreateDirectory(string path, CancellationToken cancellationToken = default);

        Task Save(Stream contentStream, string path, IDictionary<string, string> metadata = default, CancellationToken cancellationToken = default);

        Task Copy(string source, string destination, IDictionary<string, string> metadata = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Attempts to download the filed at the specified path.
        /// </summary>
        /// <param name="path">The path where the file is located.</param>
        /// <param name="fileName">Optional file name replacement for the download.</param>
        Task<DownloadResult> Download(string path, string fileName);
    }

    public class DownloadResult
    {
        public Stream Stream { get; }
        public string ContentType { get; }
        public string Url { get; }
        public string Error { get; }

        public static DownloadResult NotFound = new DownloadResult();

        private DownloadResult()
        {
        }

        private DownloadResult(string url, string error)
        {
            Url = url;
            Error = error;
        }

        private DownloadResult(Stream stream, string contentType)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            ContentType = contentType ?? HttpConstants.BinaryMimeType;
        }

        public static DownloadResult FromUrl(string url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            return new DownloadResult(url, default(string));
        }

        public static DownloadResult FromError(string error)
        {
            if (error is null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            return new DownloadResult(default(string), error);
        }

        public static DownloadResult FromStream(Stream stream, string contentType = null)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return new DownloadResult(stream, contentType);
        }
    }

    public static class FileSystemExtensions
    {
        public static async Task Save(this IFileSystem fileSystem, byte[] data, string path,
            IDictionary<string, string> metadata = default, CancellationToken cancellationToken = default)
        {
            if (fileSystem is null)
            {
                throw new System.ArgumentNullException(nameof(fileSystem));
            }

            if (data is null)
            {
                throw new System.ArgumentNullException(nameof(data));
            }

            if (path is null)
            {
                throw new System.ArgumentNullException(nameof(path));
            }

            using var memoryStream = new MemoryStream(data);
            await fileSystem.Save(memoryStream, path, metadata, cancellationToken).ConfigureAwait(false);
        }
    }
}
