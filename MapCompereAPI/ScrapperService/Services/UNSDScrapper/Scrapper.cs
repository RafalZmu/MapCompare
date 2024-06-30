
using Microsoft.Playwright;

namespace ScrapperService.Services.UNSDScrapper
{
    public class Scrapper
    {
        public static async Task<string> Scrape(string url)
        {
            //Get all the titles of the data from the page. return
            var http = new HttpClient();
            await GetDatasetsTitles(url, http);

            //Ask GTP4All service which title aligns best with the query

            //Download the data from the page




            return "";
        }

        public static async Task<List<string>> GetDatasetsTitles(string url, HttpClient http)
        {

            // Initialize Playwright
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();

            await page.GotoAsync(url);

            var titles = await page.QuerySelectorAllAsync(".Result h2");

            List<string> Content = new();
            foreach (var title in titles)
            {
                var innerTextH2 = await title.InnerTextAsync();

                Content.Add(innerTextH2);

            }

            return Content;
        }
    }
}
