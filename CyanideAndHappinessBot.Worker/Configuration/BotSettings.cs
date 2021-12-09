namespace CyanideAndHappinessBotWorker.Configuration;

public class BotSettings
{
    public string PageId { get; set; }

    public string PageToken { get; set; }

    public string Schedule { get; set; }

    public int WorkerDelayInSeconds { get; set; } = 5;

    public bool SaveGeneratedComics { get; set; } = false;
}
