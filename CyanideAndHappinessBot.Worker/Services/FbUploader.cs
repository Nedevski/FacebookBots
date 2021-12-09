using CyanideAndHappinessBotWorker.Configuration;

namespace CyanideAndHappinessBotWorker.Services;

public class FbUploader
{
    private HttpClient _fb;
    private ComicGenerator _generator;
    private BotSettings _botSettings;

    public FbUploader(IConfiguration configuration)
    {
        _botSettings = configuration.GetSection(nameof(BotSettings)).Get<BotSettings>();

        _fb = new HttpClient();
        _fb.BaseAddress = new Uri("https://graph.facebook.com/");

        _generator = new ComicGenerator();
    }

    public async Task<string> GenerateAndUpload()
    {
        Console.WriteLine(_botSettings.PageId);

        // Create a comic
        var comicPath = await _generator.Create();

        // Create a FB Post with the comic
        var formData = new MultipartFormDataContent();

        HttpResponseMessage fbResponse;
        using var fs = File.OpenRead(comicPath);
        {
            formData.Add(new StreamContent(fs), "source", "source.jpg");

            string url = $"{_botSettings.PageId}/photos?access_token={_botSettings.PageToken}";
            fbResponse = await _fb.PostAsync(url, formData);

            fs.Close();
        }

        var result = await fbResponse.Content.ReadAsStringAsync();

        // Delete the comic
        File.Delete(comicPath);

        return result;
    }
}
