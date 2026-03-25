using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gem.BLL.IServices
{
    public interface IImgGenerationService
    {
        Task<Image> GenerateImageAsync(string prompt, int width = 512, int height = 512, CancellationToken cancellationToken = default);
    }
}
