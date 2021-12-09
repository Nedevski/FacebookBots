using System.Drawing.Imaging;

namespace CyanideAndHappinessBotWorker.Services;

public class FacebookService
{
    private const string FB_URL = "https://graph.facebook.com/";

    private HttpClient _client;
    private ComicGeneratorService _comicGenerator;
    private BotSettings _botSettings;

    public FacebookService(IOptions<BotSettings> botSettings, ComicGeneratorService comicGenerator)
    {
        _botSettings = botSettings.Value;

        _client = new HttpClient();
        _client.BaseAddress = new Uri(FB_URL);

        _comicGenerator = comicGenerator;
    }

    public async Task<string> GenerateAndUpload()
    {
        // Create a comic
        var comic = await _comicGenerator.Create();

        // Create a FB Post with the comic
        var formData = new MultipartFormDataContent();

        HttpResponseMessage fbResponse;
        using MemoryStream ms = new();
        {
            comic.Save(ms, ImageFormat.Jpeg);

            formData.Add(new ByteArrayContent(ms.ToArray()), "source", "source.jpg");

            string url = $"{_botSettings.PageId}/photos?access_token={_botSettings.PageToken}";
            fbResponse = await _client.PostAsync(url, formData);

            ms.Close();
        }

        var result = await fbResponse.Content.ReadAsStringAsync();

        return result;
    }
}
