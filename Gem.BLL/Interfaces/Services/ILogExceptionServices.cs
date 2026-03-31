using Gem.COMMON.ResultModel;
using Gem.DAL.Domain;

namespace Gem.BLL.Interfaces.Services
{
    public interface ILogExceptionServices
    {
        Task<ResModel> LogExceptionToDB(ExceptionLog exception);
    }
}
