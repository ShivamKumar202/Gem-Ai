using Gem.COMMON.Enum;
using Microsoft.AspNetCore.Http;

namespace Gem.COMMON.ViewModel.Messages
{
    public class VMAddMessage
    {
        public string Content { get; set; }
        public string Model { get; set; }
        public ChatRoles Role { get; set; }
        public string ConversationId { get; set; }
        public List<IFormFile> Files { get; set; }
    }
}
