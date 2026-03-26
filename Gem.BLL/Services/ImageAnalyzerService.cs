using Gem.BLL.Common.Utility;
using Gem.BLL.Interfaces.Services;
using Gem.COMMON.Enum;
using Gem.COMMON.ResultModel;
using Gem.COMMON.Utility;
using Gem.COMMON.ViewModel.Response;
using Gem.COMMON.ViewModel.Token_Usage;
using Gem.DAL.Domain;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Gem.BLL.Services
{
    public class ImageAnalyzerService(IConfiguration configuration) : IImageAnalyzerService
    {
        private readonly Client _client = new(apiKey: configuration["GoogleAI:ApiKey"]);
        private readonly string _model = configuration["GoogleAI:Model"];

        public async Task<ResModel<VMApiResponse>> AnalyzeAsync(List<Message> messages, List<IFormFile> files, string prompt, CancellationToken ct = default)
        {
            if ((files == null || files.Count == 0) && string.IsNullOrWhiteSpace(prompt))
            {
                return SD.CreateResponse<VMApiResponse>(null, false, "No input provided", (int)StatusCode.BadRequest);
            }

            try
            {
                var contents = BuildContents(messages, files, prompt, ct);

                var response = await _client.Models.GenerateContentAsync(
                    model: _model,
                    contents: contents,
                    cancellationToken: ct);

                var vmResponse = new VMApiResponse
                {
                    Content = ExtractText(response),
                    MetaData = ExtractTokenUsage(response?.UsageMetadata)
                };

                return SD.CreateResponse(vmResponse, true, Messages.ANALYSIS_SUCCESS, (int)StatusCode.OK);
            }
            catch (OperationCanceledException)
            {
                return SD.CreateResponse<VMApiResponse>(null, false, "Request cancelled", 499);
            }
            catch (Exception ex)
            {
                return SD.CreateResponse<VMApiResponse>(null, false, ex.Message, (int)StatusCode.InternalServerError);
            }
        }

        public async Task GenerateImageAsync(string prompt, int width = 512, int height = 512, CancellationToken cancellationToken = default)
        {
            var model = "gemini-2.5-flash-image";
            var contents = new List<Content>
        {
            new Content
            {
                Role = "user",
                Parts = new List<Part>
                {
                    new Part { Text = "INSERT_INPUT_HERE" },
                }
            },
        };



            var config = new GenerateContentConfig
            {
                ResponseModalities = new List<string>
            {
                "IMAGE",
                "TEXT"
            },
            };

            int fileIndex = 0;
            await foreach (var chunk in _client.Models.GenerateContentStreamAsync(model, contents, config))
            {
                if (chunk.Candidates == null || chunk.Candidates.Count == 0 ||
                    chunk.Candidates[0].Content?.Parts == null)
                {
                    continue;
                }
                var part = chunk.Candidates[0].Content.Parts[0];
                if (part.InlineData?.Data != null)
                {
                    var fileName = $"ENTER_FILE_NAME_{fileIndex}";
                    fileIndex++;
                    var inlineData = part.InlineData;
                    var dataBuffer = inlineData.Data;
                    var fileExtension = GetFileExtension(inlineData.MimeType);
                    SaveBinaryFile($"{fileName}{fileExtension}", dataBuffer);
                }
                else
                {
                    Console.WriteLine(chunk);
                }
            }
        }

        #region Methods
        private static List<Content> BuildContents(List<Message> messages, List<IFormFile> files, string prompt, CancellationToken ct)
        {
            var contents = new List<Content>();

            if (messages != null)
            {
                foreach (var msg in messages.OrderBy(m => m.CreatedAt))
                {
                    contents.Add(new Content
                    {
                        Role = msg.Role.ToString().ToLower(),
                        Parts =
                        [
                            new() { Text = msg.Content }
                        ]
                    });
                }
            }

            var currentParts = new List<Part>();

            if (!string.IsNullOrWhiteSpace(prompt))
                currentParts.Add(new Part { Text = prompt });

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    using var ms = new MemoryStream();
                    file.CopyTo(ms);

                    currentParts.Add(new Part
                    {
                        InlineData = new Blob
                        {
                            MimeType = file.ContentType,
                            Data = ms.ToArray()
                        }
                    });
                }
            }

            contents.Add(new Content
            {
                Role = ChatRoles.User.ToString().ToLower(),
                Parts = currentParts
            });

            return contents;
        }

        private static string ExtractText(GenerateContentResponse response)
        {
            if (response?.Candidates == null)
                return string.Empty;

            var sb = new StringBuilder();

            foreach (var candidate in response.Candidates)
            {
                var parts = candidate.Content?.Parts;
                if (parts == null) continue;

                foreach (var part in parts)
                {
                    if (!string.IsNullOrWhiteSpace(part.Text))
                        sb.AppendLine(part.Text);
                }
            }

            return sb.ToString().Trim();
        }

        private static VMAddTokenUsage ExtractTokenUsage(GenerateContentResponseUsageMetadata meta)
        {
            if (meta == null)
                return new VMAddTokenUsage();

            return new VMAddTokenUsage
            {
                PromptTokenCount = meta.PromptTokenCount,
                CachedContentTokenCount = meta.CachedContentTokenCount,
                ThoughtsTokenCount = meta.ThoughtsTokenCount,
                CandidatesTokenCount = meta.CandidatesTokenCount,
                ToolUsePromptTokenCount = meta.ToolUsePromptTokenCount,
                TotalTokenCount = meta.TotalTokenCount
            };
        }

        static string GetFileExtension(string mimeType)
        {
            return mimeType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "audio/wav" => ".wav",
                "audio/mpeg" => ".mp3",
                _ => ".bin"
            };
        }

        static void SaveBinaryFile(string fileName, byte[] data)
        {
            System.IO.File.WriteAllBytes(fileName, data);
            Console.WriteLine($"File saved to: {fileName}");
        }
        #endregion
    }
}