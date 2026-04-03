using Gem.COMMON.ResultModel;
using Gem.COMMON.ViewModel.Attachement;

namespace Gem.BLL.Interfaces.Services
{
    public interface IAttachementService
    {
        Task<ResModel> AddAttachmentsAsync(List<VMAddAttachment> vmAddAttachment);
    }
}
