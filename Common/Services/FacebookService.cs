using Common.Configuration;

using Microsoft.Extensions.Options;

using System.Drawing;
using System.Drawing.Imaging;

namespace Common.Services
{
    public class FacebookService
    {
        private const string FB_URL = "https://graph.facebook.com/";

        private HttpClient _client;
        private BaseBotSettings _baseBotSettings;

        public FacebookService(IOptions<BaseBotSettings> baseBotSettings)
        {
            _baseBotSettings = baseBotSettings.Value;

            _client = new HttpClient();
            _client.BaseAddress = new Uri(FB_URL);
        }

        protected string ImagePostPath => $"{_baseBotSettings.PageId}/photos?access_token={_baseBotSettings.PageToken}";

        public async Task<string> UploadImage(Bitmap image)
        {
            // Create a FB Post with the image
            var formData = new MultipartFormDataContent();

            HttpResponseMessage fbResponse;
            using MemoryStream ms = new();
            {
                image.Save(ms, ImageFormat.Jpeg);

                formData.Add(new ByteArrayContent(ms.ToArray()), "source", "source.jpg");

                fbResponse = await _client.PostAsync(ImagePostPath, formData);

                ms.Close();
            }

            var result = await fbResponse.Content.ReadAsStringAsync();

            return result;
        }

        public async Task<string> UploadImage(string imageUrl)
        {
            var imgBytes = await _client.GetAsync(imageUrl);
            var imgStream = await imgBytes.Content.ReadAsStreamAsync();

            // Create a FB Post with the comic
            var formData = new MultipartFormDataContent();
            formData.Add(new StreamContent(imgStream), "source", "source.jpg");

            HttpResponseMessage fbResponse = await _client.PostAsync(ImagePostPath, formData);

            var result = await fbResponse.Content.ReadAsStringAsync();

            return result;
        }
    }
}
