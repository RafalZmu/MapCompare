using CsvHelper.Configuration.Attributes;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using ScrapperService.Helpers;
using System.Reflection.Metadata.Ecma335;

namespace ScrapperService.Services.WebScrapper
{
    public class DataScrapper
    {
        private static readonly HttpClient client = new HttpClient(); // Reuse HttpClient

        public async Task<string> ScrapData(string keyword, string description)
        {
            var url = UrlCreator.CreateBraveUrl(keyword +" Per country numerical data");
            var websiteLinks = ScrapeWebsiteLink(url);

            string bestFittingWebsite = await GetBestWebsite(websiteLinks);

            var mdSourceWebsite = GetMdWebsite(websiteLinks[0]);

            return mdSourceWebsite;
        }

        private async Task<string> GetBestWebsite(List<string> websiteLinks)
        {
            var synonyms = GetDataFromJSON("synonymsWithCountry");
            synonyms = synonyms.Where(x => x.Key.Length >= 4).ToDictionary(x => x.Key, x => x.Value);

            var linksCountriesCount = new Dictionary<string, int>();
            var scrapTime = new List<string>();
            var tasks = new List<Task>();

            // Set a short timeout for HTTP requests to prevent long delays
            client.Timeout = TimeSpan.FromSeconds(10);

            foreach (var link in websiteLinks)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var countriesFound = 0;
                    string website = "";
                    try
                    {
                        // Make an HTTP request with a timeout.
                        website = await client.GetStringAsync(link);
                    }
                    catch (Exception ex)
                    {
                        // Handle errors gracefully
                        Console.WriteLine($"Error fetching {link}: {ex.Message}");
                        return;
                    }

                    // Count countries in website content
                    foreach (var country in synonyms.Keys)
                    {
                        if (website.Contains(country, StringComparison.CurrentCultureIgnoreCase))
                        {
                            countriesFound++;
                        }
                    }

                    lock (linksCountriesCount)  // Use a lock for thread safety when modifying shared dictionary
                    {
                        linksCountriesCount[link] = countriesFound;
                    }
                }));
            }

            await Task.WhenAll(tasks); // Wait for all tasks to complete

            return linksCountriesCount.OrderByDescending(x => x.Value).FirstOrDefault().Key; // Return the best website link
        }

        public Dictionary<string, string> GetDataFromJSON(string fileName)
        {
            Dictionary<string, string> data = new();
            string dataPath = AppDomain.CurrentDomain.BaseDirectory + $"\\Assets\\{fileName}.json";
            var dataJson = File.ReadAllText(dataPath);
            var dataDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(dataJson);
            return dataDict;
        }

        public static List<string> ScrapeWebsiteLink(string url)
        {
            var httpClient = new HttpClient();
            var text = httpClient.GetStringAsync(url).Result;
            var html = new HtmlDocument();
            html.LoadHtml(text);
            List<string> hrefs = new List<string>();
            var links = html.DocumentNode.SelectNodes("//a[contains(@class, 'svelte-yo6adg') and contains(@class, 'heading-serpresult')]");
            foreach (var node in links)
            {
                hrefs.Add(node.GetAttributeValue("href", string.Empty));
            }

            return hrefs;
        }
        private string GetMdWebsite(string websiteLink)
        {
            var httpClient = new HttpClient();
            var md = httpClient.GetStringAsync("https://r.jina.ai/"+websiteLink).Result;
            return md;
        }
    }
}
