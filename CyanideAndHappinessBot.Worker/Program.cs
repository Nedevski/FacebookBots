using CyanideAndHappinessBotWorker;
using CyanideAndHappinessBotWorker.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Register services
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
