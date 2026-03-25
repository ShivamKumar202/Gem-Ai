using Gem.BLL.Common.Utility;
using Gem.BLL.Interfaces.Services;
using Gem.COMMON.Enum;
using Gem.COMMON.ResultModel;
using Gem.COMMON.Utility;
using Gem.COMMON.ViewModel.Prompt;
using Gem.DAL.Domain;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;

namespace Gem.BLL.Services
{
    public class ImageAnalyzerService(IConfiguration configuration) : IImageAnalyzerService
    {
        private readonly Client _client = new(apiKey: configuration["GoogleAI:ApiKey"]);

        public async Task<ResModel<string>> AnalyzeAsync(List<Message> messages,List<VMContentInput> vmContentInput, string prompt)
        {
            var contents = new List<Content>();

            foreach (var msg in messages.OrderBy(m => m.CreatedAt))
            {
                contents.Add(new Content
                {
                    Role = msg.Role.ToString().ToLower(),
                    Parts = [new Part { Text = msg.Content }]
                });
            }

            var currentParts = new List<Part>();

            if (!string.IsNullOrWhiteSpace(prompt))
                currentParts.Add(new Part { Text = prompt });

            if (vmContentInput != null)
            {
                foreach (var content in vmContentInput)
                {
                    currentParts.Add(new Part
                    {
                        InlineData = new Blob
                        {
                            MimeType = content.MimeType,
                            Data = content.Data
                        }
                    });
                }
            }

            contents.Add(new Content
            {
                Role = "user",
                Parts = currentParts
            });

            var response = await _client.Models.GenerateContentAsync(
                model: "gemini-1.5-flash",
                contents: contents
            );

            var resultText = new StringBuilder();

            if (response.Candidates != null)
            {
                foreach (var candidate in response.Candidates)
                {
                    if (candidate.Content?.Parts != null)
                        foreach (var part in candidate.Content.Parts)
                        {
                            if (!string.IsNullOrWhiteSpace(part.Text))
                                resultText.AppendLine(part.Text);
                        }
                }
            }

            return SD.CreateResponse(resultText.ToString().Trim(),true,"Image",(int)StatusCode.OK);
        }
    }
}
