namespace Gem.COMMON.ViewModel.Token_Usage
{
    public class VMAddTokenUsage
    {
        public string MessageId { get; set; }
        public int? CachedContentTokenCount { get; set; }
        public int? CandidatesTokenCount { get; set; }
        public int? PromptTokenCount { get; set; }
        public int? ThoughtsTokenCount { get; set; }
        public int? ToolUsePromptTokenCount { get; set; }
        public int? TotalTokenCount { get; set; }
    }
}
