using Gem.API.Helpers.Filters;
using Gem.API.Helpers.Plugins;
using Gem.API.Helpers.Utility;
using Gem.BLL.Services;
using Gem.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToImage;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<GemContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers and Swagger
builder.Services.AddControllers(options => options.Filters.Add<ErrorHandlingFilter>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register your services
builder.Services.AddServices();

// 2. Register the Plugins
// We register these as singletons so the Kernel can find them.
builder.Services.AddSingleton<ImagePlugin>();
builder.Services.AddSingleton<AudioPlugin>();


// 3. Create and Register the Kernel (The Brain)
builder.Services.AddTransient<Kernel>(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();

    // Add Gemini Chat Completion
    kernelBuilder.AddGoogleAIGeminiChatCompletion(
        modelId: builder.Configuration["GoogleAI:Model"],
        apiKey: builder.Configuration["GoogleAI:ApiKey"]
    );

    // Add the Plugins from the Service Provider
    kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<ImagePlugin>());
    kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<AudioPlugin>());


    return kernelBuilder.Build();
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