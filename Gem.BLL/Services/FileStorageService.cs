using Gem.BLL.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Gem.BLL.Services
{
    public class FileStorageService(IConfiguration config) : IFileStorageService
    {
        private readonly string _basePath = config["FileStorage:BasePath"] ?? throw new ArgumentNullException(nameof(config), "File storage base path is not configured");

        public async Task<string> SaveFileAsync(string fileName, byte[] data, string mimeType)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name is required", nameof(fileName));

            if (data is null || data.Length == 0)
                throw new ArgumentException("File data is empty", nameof(data));

            EnsureBasePathExists();

            try
            {
                var extension = Path.GetExtension(fileName);
                if (string.IsNullOrWhiteSpace(extension))
                    extension = ".bin";

                var uniqueName = $"{Guid.NewGuid()}{extension}";
                var fullPath = GetFullPath(uniqueName);

                await File.WriteAllBytesAsync(fullPath, data).ConfigureAwait(false);

                return uniqueName;
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to save file", ex);
            }
        }

        public async Task<byte[]> GetFileAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Invalid file name", nameof(fileName));

            var fullPath = GetFullPath(fileName);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("File not found", fullPath);

            try
            {
                return await File.ReadAllBytesAsync(fullPath).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to read file", ex);
            }
        }

        #region Private Helpers

        private void EnsureBasePathExists()
        {
            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);
        }

        private string GetFullPath(string fileName) => Path.Combine(_basePath, fileName);

        #endregion
    }
}
