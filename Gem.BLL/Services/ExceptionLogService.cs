using Gem.BLL.Common.Utility;
using Gem.BLL.Interfaces.Services;
using Gem.COMMON.Enum;
using Gem.COMMON.ResultModel;
using Gem.COMMON.Utility;
using Gem.DAL;
using Gem.DAL.Domain;

namespace Gem.BLL.Services
{
    public class ExceptionLogService(GemContext context): ILogExceptionServices
    {
        private readonly GemContext _context = context;

        public async Task<ResModel> LogExceptionToDB(ExceptionLog exception)
        {
            await _context.ExceptionLog.AddAsync(exception);
            _context.SaveChanges();
            return SD.CreateResponse(true, Messages.EXCEPTION_LOGSUCCESS, (int)StatusCode.OK);
        }
    }
}