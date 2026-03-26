using Gem.BLL.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Gem.BLL.Services
{
    public class FileStorageService (IConfiguration config) : IFileStorageService
    {
        private readonly string _basePath = config["FileStorage:BasePath"];

        public async Task<string> SaveFileAsync(string fileName, byte[] data, string mimeType)
        {
            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name is required");

            if (data == null || data.Length == 0)
                throw new ArgumentException("File data is empty");

            try
            {
                var extension = Path.GetExtension(fileName);

                if (string.IsNullOrWhiteSpace(extension))
                    extension = ".bin";

                var uniqueName = $"{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(_basePath, uniqueName);

                await File.WriteAllBytesAsync(fullPath, data);

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
                throw new ArgumentException("Invalid file name");

            var fullPath = Path.Combine(_basePath, fileName);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("File not found");

            try
            {
                return await File.ReadAllBytesAsync(fullPath);
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to read file", ex);
            }
        }
    }
}