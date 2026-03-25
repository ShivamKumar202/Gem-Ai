using Gem.BLL.IServices;
using Gem.COMMON.Enum;
using Gem.COMMON.ResultModel;
using Gem.DAL.Domain;

namespace Gem.API.Helpers.Utility
{
    public class Logger(ILogExceptionServices logException, IWebHostEnvironment webHostEnvironment)
    {
        private readonly ILogExceptionServices _logException = logException;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

        public async virtual Task<ResModel> LogError(Exception ex)
        {
            ExceptionLog exceptionLog = new()
            {
                Message = ex.Message,
                Source = ex.Source,
                StackTrace = ex.StackTrace,
                ActivatedOn = DateTime.Now,
            };

            var folderName = Path.Combine("Files", "Exception", ex.Source);
            var pathToSave = Path.Combine(_webHostEnvironment.WebRootPath, folderName);

            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            var filePath = Path.Combine(pathToSave, "exceptions.txt");

            using (StreamWriter writer = new(filePath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date: " + DateTime.Now.ToString());
                writer.WriteLine();

                Exception exception = ex;

                while (exception is not null)
                {
                    writer.WriteLine(exception.GetType().FullName);
                    writer.WriteLine("Message: " + exception.Message);
                    writer.WriteLine("StackTrace: " + exception.StackTrace);
                    exception = exception.InnerException;
                }
            }
            await _logException.LogExceptionToDB(exceptionLog);
            return new ResModel
            {
                Message = ex.Message.ToString(),
                Errors = [ex.Message],
                StatusCode = Convert.ToInt32(StatusCode.BadRequest),
                Success = false
            };
        }
    }
}
