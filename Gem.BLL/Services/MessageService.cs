using Gem.BLL.Common.Utility;
using Gem.BLL.Interfaces.Services;
using Gem.COMMON.Enum;
using Gem.COMMON.ResultModel;
using Gem.COMMON.Utility;
using Gem.COMMON.ViewModel.Attachement;
using Gem.COMMON.ViewModel.Messages;
using Gem.DAL;
using Gem.DAL.Domain;
using Microsoft.EntityFrameworkCore;

namespace Gem.BLL.Services
{
    public class MessageService(GemContext context, IAttachementService attachmentService) : IMessageService
    {
        private readonly GemContext _context = context;
        private readonly IAttachementService _attachmentService = attachmentService;

        public async Task<ResModel<string>> AddMessageAsync( VMAddMessage vmAddMessage, CancellationToken cancellationToken = default)
        {
            if (vmAddMessage == null)
                return SD.CreateResponse<string>(null, false, "Invalid request", (int)StatusCode.BadRequest);

            if (string.IsNullOrWhiteSpace(vmAddMessage.ConversationId))
                return SD.CreateResponse<string>(null, false, "ConversationId is required", (int)StatusCode.BadRequest);

            if (string.IsNullOrWhiteSpace(vmAddMessage.Content) && (vmAddMessage.Files == null || vmAddMessage.Files.Count == 0))
            
                return SD.CreateResponse<string>(null, false, "Message content or file required", (int)StatusCode.BadRequest);
            

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var message = new Message
                {
                    ThreadId = vmAddMessage.ConversationId,
                    Content = vmAddMessage.Content,
                    Role = vmAddMessage.Role,
                    Model = vmAddMessage.Model
                };

                await _context.Message.AddAsync(message, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                if (vmAddMessage.Files != null && vmAddMessage.Files.Count > 0)
                {
                    var attachmentDtos = vmAddMessage.Files.Select(f => new VMAddAttachment
                    {
                        MessageId = message.Id,
                        File = f,
                        FileName = f.FileName,
                        MimeType = f.ContentType
                    }).ToList();

                    var attachmentResult = await _attachmentService.AddAttachmentsAsync(attachmentDtos);

                    if (!attachmentResult.Success)
                        return SD.CreateResponse<string>( null, false, attachmentResult.Message,attachmentResult.StatusCode);
                }

                await transaction.CommitAsync(cancellationToken);

                return SD.CreateResponse(  message.Id,true,Messages.MESSAGE_SUCCESS,(int)StatusCode.OK);
            }
            catch (OperationCanceledException)
            {
                await transaction.RollbackAsync(cancellationToken);

                return SD.CreateResponse<string>(null, false, "Request cancelled", 499);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                return SD.CreateResponse<string>(
                    null,
                    false,
                    ex.Message,
                    (int)StatusCode.InternalServerError);
            }
        }

        public Task<ResModel> DeleteMessageAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Message>> GetLatestMessagesAsync( string threadId, int size, CancellationToken cancellationToken)
        {
            return await _context.Message .AsNoTracking() .Where(m => m.ThreadId == threadId).OrderByDescending(m => m.CreatedAt).Take(size) .OrderBy(m => m.CreatedAt).ToListAsync(cancellationToken);
        }
    }
}