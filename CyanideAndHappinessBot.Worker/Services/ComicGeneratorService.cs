using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

using System.Drawing;
using System.Drawing.Imaging;

namespace CyanideAndHappinessBotWorker.Services;

public class ComicGeneratorService
{
    private BotSettings _botSettings;
    private HttpClient _client;

    public ComicGeneratorService(IOptions<BotSettings> botSettings)
    {
        _botSettings = botSettings.Value;

        _client = new HttpClient();
    }

    public async Task<Bitmap> GetNewComic()
    {
        string imgXpath = "//div[contains(@class, 'Panel__Container')]/img";

        var driverOptions = new ChromeOptions();
        driverOptions.AddArgument("headless");

        using WebDriver driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory, driverOptions);

        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        driver.Navigate().GoToUrl("https://explosm.net/rcg");

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

        wait.Until(x => x.FindElement(By.XPath(imgXpath)));

        var imageUrls = driver
            .FindElements(By.XPath(imgXpath))
            .Select(x => x.GetDomAttribute("src"))
            .ToArray();

        driver.Close();

        if (imageUrls.Any(n => n is null || n == string.Empty) || imageUrls.Count() != 3)
        {
            Console.WriteLine("Error");
            throw new ArgumentException("Unable to fetch comic images");
        }

        List<Bitmap> images = new();

        for (int i = 0; i < imageUrls.Count(); i++)
        {
            var imgBytes = await _client.GetAsync(imageUrls[i]);
            var imgStream = await imgBytes.Content.ReadAsStreamAsync();
            images.Add(new Bitmap(imgStream));
        }

        var height = images.First().Height;
        var width = images.First().Width;

        Bitmap comic = new Bitmap(width * 3, height);

        using var canvas = Graphics.FromImage(comic);
        {
            for (int i = 0; i < imageUrls.Count(); i++)
            {
                canvas.DrawImage(images[i], width * i, 0);
            }

            canvas.Save();
        }

        if (_botSettings.SaveGeneratedComics)
        {
            Directory.CreateDirectory("generated");
            string imagePath = $"generated/{DateTime.UtcNow:yyyyMMdd-HHmmss}.jpg";
            comic.Save(imagePath, ImageFormat.Jpeg);
        }

        return comic;
    }
}
