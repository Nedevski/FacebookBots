using Common.Configuration;
using Common.Services;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NCrontab;

namespace Common
{
    public abstract class BaseWorker : BackgroundService
    {
        private BaseBotSettings _botSettings;

        private CrontabSchedule _schedule;
        private DateTime _nextRun;

        protected readonly ILogger<BaseWorker> _logger;
        protected FacebookService _fbService;

        public BaseWorker(
            ILogger<BaseWorker> logger,
            IOptions<BaseBotSettings> botSettings,
            FacebookService fbService)
        {
            _logger = logger;
            _fbService = fbService;

            _botSettings = botSettings.Value;

            _schedule = CrontabSchedule.Parse(_botSettings.Schedule, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
        }

        protected abstract Task ProcessAsync();

        protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
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
    }
}
