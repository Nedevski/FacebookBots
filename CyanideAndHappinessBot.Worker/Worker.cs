using CyanideAndHappinessBotWorker.Services;
using NCrontab;

namespace CyanideAndHappinessBotWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private CrontabSchedule _schedule;
    private DateTime _nextRun;

    private FbUploader _fbUploader;

    private string Schedule => "0 0 */6 * * *"; //Runs every 6 hours

    public Worker(ILogger<Worker> logger, FbUploader fbUploader)
    {
        _logger = logger;

        _schedule = CrontabSchedule.Parse(Schedule, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);

        _fbUploader = fbUploader;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextrun = _schedule.GetNextOccurrence(now);
            if (now > _nextRun)
            {
                await ProcessAsync();
                _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            }
            await Task.Delay(5000, stoppingToken); //5 seconds delay

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcessAsync()
    {
        Console.WriteLine("Generating comic...");
        Console.WriteLine(await _fbUploader.GenerateAndUpload());
        Console.WriteLine("Done!");
    }
}
