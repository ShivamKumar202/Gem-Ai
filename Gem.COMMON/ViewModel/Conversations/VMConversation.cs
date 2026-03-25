using System;
using System.Collections.Generic;
using System.Text;

namespace Gem.COMMON.ViewModel.Conversations
{
    public class VMConversation
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
