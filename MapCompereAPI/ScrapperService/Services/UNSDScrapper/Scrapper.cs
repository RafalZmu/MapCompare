using Microsoft.Playwright;
using ScrapperService.Connectors;

namespace ScrapperService.Services.UNSDScrapper
{
    public class Scrapper
    {
        private ILLMServiceConnector _LLMConnector;

        public Scrapper(ILLMServiceConnector LLMConnector)
        {
            _LLMConnector = LLMConnector;
        }
        public static async Task<string> Scrape(string url, string query, ILLMServiceConnector LLMService)
        {
            //Get all the titles of the data from the page. return
            var http = new HttpClient();
            var Titles = await GetDatasetsTitles(url, http);

            //Ask GTP4All service which title aligns best with the query
            var MostRelevantTitle = await LLMService.GetPrediction(Titles.ToString(), $"Which one of provided Titles matches best to this query: {query}. Return only the number that corresponst to the most relevant query");

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
