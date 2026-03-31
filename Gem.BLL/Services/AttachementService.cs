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
            if (attachments is null || attachments.Count == 0)
                return SD.CreateResponse(false, Messages.NO_ATTACHMENT, (int)StatusCode.BadRequest);

            var savedAttachments = new List<Attachment>();

            try
            {
                foreach (var vmFile in attachments)
                {
                    if (string.IsNullOrWhiteSpace(vmFile.FileName))
                        return SD.CreateResponse(false, "File name is required", (int)StatusCode.BadRequest);

                    var fileData = await ExtractFileDataAsync(vmFile).ConfigureAwait(false);
                    if (fileData is null || fileData.Length == 0)
                        return SD.CreateResponse(false, "Invalid or empty file", (int)StatusCode.BadRequest);

                    var fileUrl = await _fileStorageService
                        .SaveFileAsync(vmFile.FileName, fileData, vmFile.MimeType)
                        .ConfigureAwait(false);

                    var attachment = new Attachment
                    {
                        MessageId = vmFile.MessageId,
                        FileName = vmFile.FileName,
                        MimeType = vmFile.MimeType,
                        Url = fileUrl,
                        Embedding = vmFile.Embedding
                    };

                    await _context.Attachment.AddAsync(attachment).ConfigureAwait(false);
                    savedAttachments.Add(attachment);
                }

                await _context.SaveChangesAsync().ConfigureAwait(false);

                return SD.CreateResponse(true, Messages.ATTACHMENT_SUCCESS, (int)StatusCode.OK);
            }
            catch (Exception ex)
            {
                return SD.CreateResponse(false, ex.Message, (int)StatusCode.InternalServerError);
            }
        }

        #region Private Helpers

        private static async Task<byte[]> ExtractFileDataAsync(VMAddAttachment vmFile)
        {
            if (vmFile.File is not null)
            {
                using var ms = new MemoryStream();
                await vmFile.File.CopyToAsync(ms).ConfigureAwait(false);
                return ms.ToArray();
            }

            return vmFile.Data;
        }

        #endregion
    }
}
