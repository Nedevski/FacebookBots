using CyanideAndHappinessBotWorker.Configuration;
using CyanideAndHappinessBotWorker.Services;
using Microsoft.Extensions.Options;
using NCrontab;

namespace CyanideAndHappinessBotWorker;

public class Worker : BackgroundService
{
    private readonly BotSettings _botSettings;
    private readonly ILogger<Worker> _logger;

    private CrontabSchedule _schedule;
    private DateTime _nextRun;

    private FbUploader _fbUploader;

    public Worker(ILogger<Worker> logger, IOptions<BotSettings> botSettings, FbUploader fbUploader)
    {
        _botSettings = botSettings.Value;
        _logger = logger;

        _schedule = CrontabSchedule.Parse(_botSettings.Schedule, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);

        _fbUploader = fbUploader;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            if (DateTime.Now > _nextRun)
            {
                await ProcessAsync();

                _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            }

            await Task.Delay(_botSettings.WorkerDelayInSeconds * 1000, stoppingToken);
        }
    }

    private async Task ProcessAsync()
    {
        _logger.LogInformation("Processing started");

        var response = await _fbUploader.GenerateAndUpload();

        _logger.LogInformation(response);
    }
}
