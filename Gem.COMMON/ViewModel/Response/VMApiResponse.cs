using Gem.COMMON.ViewModel.Token_Usage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gem.COMMON.ViewModel.Response
{
    public class VMApiResponse
    {
        public string Content { get; set; }
        public VMAddTokenUsage MetaData { get;set;  }
    }
}
