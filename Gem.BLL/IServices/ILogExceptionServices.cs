using Gem.COMMON.ResultModel;
using Gem.DAL.Domain;

namespace Gem.BLL.IServices
{
    public interface ILogExceptionServices
    {
        Task<ResModel> LogExceptionToDB(ExceptionLog exception);
    }
}
