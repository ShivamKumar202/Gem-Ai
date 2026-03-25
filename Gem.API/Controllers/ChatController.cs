using Gem.BLL.Interfaces.Services;
using Gem.COMMON.ViewModel.Prompt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController(IChatService chatService) : ControllerBase
    {
        private readonly IChatService _chatService = chatService;

        //[HttpPost]
        //public async Task<IActionResult> Chat([FromBody]VMPromptRequest request)
        //{
        //    if (string.IsNullOrWhiteSpace(request.Prompt))
        //        return BadRequest("Message required");

        //    var response = await _chatService.ExecutePromptAsync( request);

        //    return Ok(new
        //    {
        //        user = request.Prompt,
        //        ai = response
        //    });
        //}
        
    }
}
