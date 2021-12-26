namespace RandomPersonBotWorker;

public class Worker : BaseWorker
{
    private const string RANDOM_PERSON_URL = "https://thispersondoesnotexist.com/image";

    public Worker(
        ILogger<Worker> logger,
        IOptions<BaseBotSettings> baseBotSettings,
        FacebookService fbService) : base(logger, baseBotSettings, fbService)
    {
    }

    protected override async Task ProcessAsync()
    {
        var response = await _fbService.UploadImage(RANDOM_PERSON_URL);

        _logger.LogInformation(response);
    }
}
