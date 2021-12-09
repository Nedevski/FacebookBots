using HtmlAgilityPack;
using System.Drawing;
using System.Drawing.Imaging;

namespace CyanideAndHappinessBotWorker.Services;

public class ComicGenerator
{
    private HttpClient _client;

    public ComicGenerator()
    {
        _client = new HttpClient();
    }

    public async Task<string> Create()
    {
        var randomComicResponse = await _client.GetAsync("https://explosm.net/rcg");

        var randomComicResult = await randomComicResponse.Content.ReadAsStringAsync();
        var html = new HtmlDocument();

        html.LoadHtml(randomComicResult);

        var imageNodes = html.DocumentNode
            .SelectNodes($"//div[@class='rcg-panels']/img")
            .Select(node => node.Attributes.FirstOrDefault(a => a.Name == "src")?.Value)
            .ToArray();

        if (imageNodes.Any(n => n is null) && imageNodes.Count() != 3)
        {
            Console.WriteLine("Error");
            throw new ArgumentException("Unable to fetch comic images");
        }

        List<Bitmap> images = new List<Bitmap>();

        for (int i = 0; i < imageNodes.Count(); i++)
        {
            var imgBytes = await _client.GetAsync(imageNodes[i]);
            var imgStream = await imgBytes.Content.ReadAsStreamAsync();
            images.Add(new Bitmap(imgStream));
        }

        var height = images.First().Height;
        var width = images.First().Width;

        using Bitmap comic = new Bitmap(width * 3, height);
        using var canvas = Graphics.FromImage(comic);

        for (int i = 0; i < imageNodes.Count(); i++)
        {
            canvas.DrawImage(images[i], width * i, 0);
        }

        canvas.Save();

        Directory.CreateDirectory("generated");
        string imagePath = $"generated/{DateTime.UtcNow:yyyyMMdd-HHmmss}.jpg";
        comic.Save(imagePath, ImageFormat.Jpeg);

        return imagePath;
    }
}
