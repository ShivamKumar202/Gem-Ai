using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.TextGeneration;
using System.Collections.ObjectModel;

namespace Gem.API.Helpers.Wrapper
{
    public class TextToChatWrapper(ITextGenerationService textService) : IChatCompletionService
    {
        private readonly ITextGenerationService _textService = textService;

        public IReadOnlyDictionary<string, object> Attributes => ReadOnlyDictionary<string, object>.Empty;

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null,CancellationToken cancellationToken = default)
        {
            var prompt = string.Join("\n", chatHistory.Select(m => $"{m.Role}: {m.Content}")) + "\nAssistant:";

            var result = await _textService.GetTextContentsAsync(prompt, executionSettings, kernel, cancellationToken);

            var chatMessages = result.Select(text => new ChatMessageContent(AuthorRole.Assistant, text.Text)).ToList();

            foreach (var message in chatMessages)
            {
                chatHistory.Add(message);
            }

            return chatMessages;
        }

        public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
