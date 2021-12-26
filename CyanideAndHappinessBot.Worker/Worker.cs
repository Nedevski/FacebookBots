using Common;
using Common.Configuration;
using Common.Services;

using CyanideAndHappinessBotWorker.Services;

using Microsoft.Extensions.Options;

namespace CyanideAndHappinessBotWorker;

public class Worker : BaseWorker
{
    private ComicGeneratorService _comicGenerator;

    public Worker(
        ILogger<Worker> logger,
        IOptions<BaseBotSettings> baseBotSettings,
        FacebookService fbService,
        ComicGeneratorService comicGenerator) : base(logger, baseBotSettings, fbService)
    {
        _comicGenerator = comicGenerator;
    }

    protected override async Task ProcessAsync()
    {
        var comic = await _comicGenerator.GetNewComic();
        var response = await _fbService.UploadImage(comic);

        _logger.LogInformation(response);
    }
}
