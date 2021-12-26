namespace Common.Configuration
{
    public class BaseBotSettings
    {
        public string PageId { get; set; }

        public string PageToken { get; set; }

        public string Schedule { get; set; }

        public int WorkerDelayInSeconds { get; set; } = 5;
    }
}
