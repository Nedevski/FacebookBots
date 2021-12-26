using Microsoft.Extensions.Options;

using RandomPersonBotWorker.Configuration;

namespace RandomPersonBotWorker.Services
{
    public class RandomPersonService
    {
        private const string FB_URL = "https://graph.facebook.com/";
        private const string RANDOM_PERSON_URL = "https://thispersondoesnotexist.com/image";

        private HttpClient _client;
        private BotSettings _botSettings;

        public RandomPersonService(IOptions<BotSettings> options)
        {
            _botSettings = options.Value;

            _client = new HttpClient();
            _client.BaseAddress = new Uri(FB_URL);
        }

        public async Task<string> GenerateAndUpload()
        {
            var imgBytes = await _client.GetAsync(RANDOM_PERSON_URL);
            var imgStream = await imgBytes.Content.ReadAsStreamAsync();

            // Create a FB Post with the comic
            var formData = new MultipartFormDataContent();

            HttpResponseMessage fbResponse;
            using MemoryStream ms = new();
            {
                formData.Add(new StreamContent(imgStream), "source", "source.jpg");

                string url = $"{_botSettings.PageId}/photos?access_token={_botSettings.PageToken}";
                fbResponse = await _client.PostAsync(url, formData);

                ms.Close();
            }

            var result = await fbResponse.Content.ReadAsStringAsync();

            return result;
        }
    }
}
