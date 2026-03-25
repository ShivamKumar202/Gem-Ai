using Gem.API.Helpers.Filters;
using Gem.API.Helpers.Utility;
using Gem.BLL.Interfaces.Services;
using Gem.BLL.Services;
using Gem.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<GemContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers and Swagger
builder.Services.AddControllers(options => options.Filters.Add<ErrorHandlingFilter>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register your services
builder.Services.AddServices();

// Create Kernel as a singleton
builder.Services.AddSingleton(sp =>
{
    var kernel = Kernel.CreateBuilder();

    var googleChatApiKey = builder.Configuration["GoogleAI:ApiKey"];
    var googleChatModel = builder.Configuration["GoogleAI:Model"];
    kernel.AddGoogleAIGeminiChatCompletion(
        modelId: googleChatModel,
        apiKey: googleChatApiKey,
        apiVersion: GoogleAIVersion.V1
    );
    return kernel.Build();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gem API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();