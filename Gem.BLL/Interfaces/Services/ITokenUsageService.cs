using Gem.COMMON.ResultModel;
using Gem.COMMON.ViewModel.Token_Usage;

namespace Gem.BLL.Interfaces.Services
{
    public interface ITokenUsageService
    {
        Task<ResModel> AddTokenUsageAsync(VMAddTokenUsage vmAddTokenUsage, CancellationToken cancellationToken = default);
    }
}
