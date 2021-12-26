global using Common;
global using Common.Configuration;
global using Common.Services;

global using Microsoft.Extensions.Options;

using RandomPersonBotWorker;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

// Register services
services.Configure<BaseBotSettings>(config.GetSection(nameof(BaseBotSettings)));

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
