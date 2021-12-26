using Microsoft.Extensions.Options;

using NCrontab;

using RandomPersonBotWorker.Configuration;
using RandomPersonBotWorker.Services;

namespace RandomPersonBotWorker;

public class Worker : BackgroundService
{
    private readonly BotSettings _botSettings;
    private readonly ILogger<Worker> _logger;

    private CrontabSchedule _schedule;
    private DateTime _nextRun;

    private RandomPersonService _rpService;

    public Worker(ILogger<Worker> logger, IOptions<BotSettings> botSettings, RandomPersonService rpService)
    {
        _botSettings = botSettings.Value;
        _logger = logger;

        _schedule = CrontabSchedule.Parse(_botSettings.Schedule, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);

        _rpService = rpService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            if (DateTime.Now > _nextRun)
            {
                try
                {
                    _logger.LogInformation("Processing started");
                    
                    await ProcessAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

                _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            }

            await Task.Delay(_botSettings.WorkerDelayInSeconds * 1000, stoppingToken);
        }
    }

    private async Task ProcessAsync()
    {
        var response = await _rpService.GenerateAndUpload();

        _logger.LogInformation(response);
    }
}
