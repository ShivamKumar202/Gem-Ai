using Gem.BLL.Common.Utility;
using Gem.BLL.Interfaces.Services;
using Gem.COMMON.Enum;
using Gem.COMMON.ResultModel;
using Gem.COMMON.Utility;
using Gem.COMMON.ViewModel.Attachement;
using Gem.DAL;
using Gem.DAL.Domain;

namespace Gem.BLL.Services
{
    public class AttachementService(GemContext context, IFileStorageService fileStorageService) : IAttachementService
    {
        private readonly IFileStorageService _fileStorageService = fileStorageService;
        private readonly GemContext _context = context;

        public async Task<ResModel> AddAttachmentsAsync(List<VMAddAttachment> attachments)
        {
            if (attachments == null || attachments.Count == 0)
                return SD.CreateResponse(false, Messages.NO_ATTACHMENT, (int)StatusCode.BadRequest);

            var savedAttachments = new List<Attachment>();

            try
            {
                foreach (var vmFile in attachments)
                {
                    if (string.IsNullOrWhiteSpace(vmFile.FileName))
                        return SD.CreateResponse(false, "File name is required", (int)StatusCode.BadRequest);

                    byte[] fileData;

                    if (vmFile.File is not null)
                    {
                        using var ms = new MemoryStream();
                        await vmFile.File.CopyToAsync(ms);
                        fileData = ms.ToArray();
                    }
                    else if (vmFile.Data is not null)
                    {
                        fileData = vmFile.Data;
                    }
                    else
                    {
                        return SD.CreateResponse(false, Messages.ATTACHMENT_INVALID, (int)StatusCode.BadRequest);
                    }

                    if (fileData.Length == 0)
                        return SD.CreateResponse(false, "Empty file", (int)StatusCode.BadRequest);

                    var fileUrl = await _fileStorageService.SaveFileAsync(
                        vmFile.FileName,
                        fileData,
                        vmFile.MimeType);

                    var attachment = new Attachment
                    {
                        MessageId = vmFile.MessageId,
                        FileName = vmFile.FileName,
                        MimeType = vmFile.MimeType,
                        Url = fileUrl,
                        Embedding = vmFile.Embedding
                    };

                    await _context.Attachment.AddAsync(attachment);
                    savedAttachments.Add(attachment);
                }

                await _context.SaveChangesAsync();

                return SD.CreateResponse(true,Messages.ATTACHMENT_SUCCESS,(int)StatusCode.OK);
            }
            catch (Exception ex)
            {
                return SD.CreateResponse(false, ex.Message,(int)StatusCode.InternalServerError);
            }
        }
    }
}