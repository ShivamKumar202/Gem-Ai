using Gem.BLL.Interfaces.Orchestrators;
using Gem.BLL.Interfaces.Services;
using Gem.BLL.Orchestrators;
using Gem.BLL.Services;

namespace Gem.API.Helpers.Utility
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<Logger>();
            services.AddHttpContextAccessor();

            // Services
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<ILogExceptionServices, ExceptionLogService>();
            services.AddScoped<IImgGenerationService, ImageGenerationService>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<IChatService, ChatService>();

            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IImageAnalyzerService, ImageAnalyzerService>();

            // Orchestors
            services.AddScoped<IAiOrchestratorService, AiOrchestratorService>();
        }
    }
}