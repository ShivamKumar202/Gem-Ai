using Gem.COMMON.Enum;

namespace Gem.COMMON.ViewModel.Messages
{
    public class VMAddMessage
    {
        public string Content { get; set; }
        public int TokenUsed { get; set; }
        public string Model { get; set; }
        public ChatRoles Role { get; set; }
        public string ConversationId { get; set; }
    }
}
