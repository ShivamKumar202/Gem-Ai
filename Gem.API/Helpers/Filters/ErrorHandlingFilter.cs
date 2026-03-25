using Gem.BLL.IServices;
using Gem.DAL.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Gem.API.Helpers.Filters
{
    public class ErrorHandlingFilter(ILogExceptionServices logException, IWebHostEnvironment webHostEnvironment) : ExceptionFilterAttribute
    {
        private readonly ILogExceptionServices _logException = logException;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

        public override async void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            var problemDetails = new ProblemDetails
            {
                Title = exception.Message,
                Status = (int)HttpStatusCode.InternalServerError
            };
            ExceptionLog excep = new()
            {
                Message = exception.Message,
                Source = exception.Source,
                StackTrace = exception.StackTrace,
                ActivatedOn = DateTime.Now,
            };

            var folderName = Path.Combine("Files/Exception/" + exception.Source);
            var pathToSave = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
            if (!Directory.Exists(pathToSave))
                Directory.CreateDirectory(pathToSave);
            
            using (FileStream fs = File.Create("exceptions"))
            {
                var pathToSaveIn = Path.Combine(pathToSave, "exceptions");
                using StreamWriter writer = new(pathToSaveIn, true);
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();

                while (exception != null)
                {
                    writer.WriteLine(exception.GetType().FullName);
                    writer.WriteLine("Message : " + exception.Message);
                    writer.WriteLine("StackTrace : " + exception.StackTrace);

                    exception = exception.InnerException;
                }
            }
            var result = await _logException.LogExceptionToDB(excep);
            if (result.Success)
            {
                context.Result = new ObjectResult(problemDetails);
                context.ExceptionHandled = true;
            }
        }
    }
}

