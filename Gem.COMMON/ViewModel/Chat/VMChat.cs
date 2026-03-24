using System;
using System.Collections.Generic;
using System.Text;

namespace Gem.COMMON.ViewModel.Chat
{
    public class VMChat
    {
        public string Prompt { get; set; }
        public Dictionary<string, string> Variables { get; set; } = null;
        public int? MaxTokens { get; set; }
        public double? Temperature { get; set; }
        public string SessionId { get; set; }
    }
}
