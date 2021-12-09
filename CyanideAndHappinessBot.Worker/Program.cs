using CyanideAndHappinessBotWorker;
using CyanideAndHappinessBotWorker.Configuration;
using CyanideAndHappinessBotWorker.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

// Register services
services.Configure<BotSettings>(config.GetSection(nameof(BotSettings)));

services.AddTransient<ComicGenerator>();
services.AddTransient<FbUploader>();

services.AddHostedService<Worker>();

// Register app dependencies
var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Service is running!");
    });
});

app.Run();
