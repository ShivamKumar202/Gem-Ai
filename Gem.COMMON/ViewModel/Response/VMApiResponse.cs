using Microsoft.SemanticKernel;
namespace Gem.COMMON.ViewModel.Response
{
    public class VMApiResponse
    {
        public string ThreadId { get; set; }
        public ChatMessageContent MetaData { get;set;  }
    }
}
