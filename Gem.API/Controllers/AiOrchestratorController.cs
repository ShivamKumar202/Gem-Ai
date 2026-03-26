using Gem.BLL.Interfaces.Orchestrators;
using Gem.COMMON.ViewModel.Prompt;
using Microsoft.AspNetCore.Mvc;

namespace Gem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiOrchestratorController(IAiOrchestratorService aiOrchestratorService) : ControllerBase
    {
        private readonly IAiOrchestratorService _aiOrchestratorService = aiOrchestratorService;

        [HttpPost("handle")]
        public async Task<IActionResult> HandleAsync([FromForm] VMPromptRequest request)
        {
            return Ok(await _aiOrchestratorService.HandleAsync(request, HttpContext.RequestAborted));
        }

        [HttpPost("generateImage")]
        public async Task<IActionResult> GenerateImageAsync([FromBody] string prompt)
        {
            return Ok(await _aiOrchestratorService.GenerateImageAsync(prompt,null));
        }
    }
}
