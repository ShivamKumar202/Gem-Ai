using Gem.BLL.Common.Utility;
using Gem.BLL.Interfaces.Services;
using Gem.COMMON.Enum;
using Gem.COMMON.ResultModel;
using Gem.COMMON.Utility;
using Gem.COMMON.ViewModel.Token_Usage;
using Gem.DAL;
using Gem.DAL.Domain;
using Microsoft.EntityFrameworkCore;

namespace Gem.BLL.Services
{
    public class TokenUsageService(GemContext context) : ITokenUsageService
    {
        private readonly GemContext _context = context;

        public async Task<ResModel> AddTokenUsageAsync(VMAddTokenUsage vmAddTokenUsage, CancellationToken cancellationToken = default)
        {
            if (vmAddTokenUsage == null)
                return SD.CreateResponse(false, "Invalid token usage request", (int)StatusCode.BadRequest);


            if (string.IsNullOrWhiteSpace(vmAddTokenUsage.MessageId))
                return SD.CreateResponse(false, "MessageId is required", (int)StatusCode.BadRequest);

            try
            {
                var messageExists = await _context.Message.AsNoTracking().AnyAsync(m => m.Id == vmAddTokenUsage.MessageId, cancellationToken);

                if (!messageExists)
                    return SD.CreateResponse( false, "Invalid MessageId", (int)StatusCode.BadRequest);
                

                var tokenUsage = new Token_Usage
                {
                    MessageId = vmAddTokenUsage.MessageId,
                    CachedContentTokenCount = vmAddTokenUsage.CachedContentTokenCount,
                    CandidatesTokenCount = vmAddTokenUsage.CandidatesTokenCount,
                    PromptTokenCount = vmAddTokenUsage.PromptTokenCount,
                    ThoughtsTokenCount = vmAddTokenUsage.ThoughtsTokenCount,
                    ToolUsePromptTokenCount = vmAddTokenUsage.ToolUsePromptTokenCount,
                    TotalTokenCount = vmAddTokenUsage.TotalTokenCount
                };

                await _context.TokenUsage.AddAsync(tokenUsage, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return SD.CreateResponse(  true, Messages.TOKEN_USAGE_SUCCESS, (int)StatusCode.OK);
            }
            catch (OperationCanceledException)
            {
                return SD.CreateResponse(false, "Request cancelled",(int)StatusCode.OperationCanceled);
            }
            catch (Exception ex)
            {
                return SD.CreateResponse( false, ex.Message,(int)StatusCode.InternalServerError);
            }
        }
    }
}