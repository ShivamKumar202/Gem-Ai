using System;
using System.Collections.Generic;
using System.Text;

namespace Gem.BLL.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(string fileName, byte[] data, string mimeType);
        Task<byte[]> GetFileAsync(string url);
    }
}
