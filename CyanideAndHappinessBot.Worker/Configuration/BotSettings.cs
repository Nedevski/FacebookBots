namespace CyanideAndHappinessBotWorker.Configuration;

internal class BotSettings
{
    public string PageId { get; set; }

    public string PageToken { get; set; }

    public string Schedule { get; set; }

    public int WorkerDelayInSeconds { get; set; }
}
