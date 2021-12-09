using CyanideAndHappinessBotWorker;
using CyanideAndHappinessBotWorker.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<ComicGenerator>();
        services.AddTransient<FbUploader>();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
