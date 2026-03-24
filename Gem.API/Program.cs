using Gem.API.Helpers.Wrapper;
using Gem.BLL.IServices;
using Gem.BLL.Services;
using Google.GenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.TextGeneration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IChatService, ChatService>();

// Semantic Kernel registration
builder.Services.AddSingleton(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder().AddGoogleAIGeminiChatCompletion(
        modelId: builder.Configuration["GoogleAI:Model"],
    apiKey: builder.Configuration["GoogleAI:ApiKey"]);

    return kernelBuilder.Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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
