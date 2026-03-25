using Gem.BLL.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageGenerationController(IImgGenerationService imgGenerationService) : ControllerBase
    {
        private readonly IImgGenerationService _imgGenerationService= imgGenerationService;

        [HttpGet]
        public async Task<IActionResult> GenerateImagefromtext(string text)
        {
            return Ok(await _imgGenerationService.GenerateImageAsync(text));
        }

    }
}
