global using Common;
global using Common.Configuration;

global using CyanideAndHappinessBotWorker.Configuration;
global using CyanideAndHappinessBotWorker.Services;

global using Microsoft.Extensions.Options;

using Common.Services;

using CyanideAndHappinessBotWorker;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

// Register services
services.Configure<BaseBotSettings>(config.GetSection(nameof(BaseBotSettings)));
services.Configure<BotSettings>(config.GetSection(nameof(BotSettings)));

services.AddTransient<ComicGeneratorService>();
services.AddTransient<FacebookService>();

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
