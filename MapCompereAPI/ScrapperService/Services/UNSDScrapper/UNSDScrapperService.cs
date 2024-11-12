using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.Playwright;
using ScrapperService.Connectors;
using ScrapperService.Models;
using System.Globalization;
using System.Linq;
using System.IO.Compression;
using System.Xml.Linq;

namespace ScrapperService.Services.UNSDScrapper
{
    public class UNSDScrapperService : IScrapperService
    {
        public UNSDScrapperService()
        {
        }
        public async Task<List<string>> GetDatasetsTitles(string url, HttpClient http)
        {
            // Initialize Playwright
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();

            await page.GotoAsync(url);

            var titles = await page.QuerySelectorAllAsync(".Result h2");

            if (titles == null)
            {
                throw new Exception("No titles found");
            }

            List<string> Content = new();
            foreach (var title in titles)
            {
                var innerTextH2 = await title.InnerTextAsync();
                Content.Add(innerTextH2);
            }

            return Content;
        }
        public async Task<string> DownloadData(string url, int dataIndexToDownload)
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();
            await page.GotoAsync(url);
            var linksToDownloadPages = await page.QuerySelectorAllAsync(".Result h2 a");

            var linkToSelectedDownloadPage = await linksToDownloadPages[dataIndexToDownload].GetAttributeAsync("href");
            linkToSelectedDownloadPage = "https://data.un.org/" + linkToSelectedDownloadPage;

            await page.GotoAsync(linkToSelectedDownloadPage);
            var downloadButton = await page.QuerySelectorAsync("a[title='Download this view of the data series']");
            await downloadButton.ClickAsync();


            await page.WaitForSelectorAsync("#downloadXmlLink");

            var downloadTask = page.RunAndWaitForDownloadAsync(async () =>
            {
                await page.ClickAsync("#downloadXmlLink");
            });

            var download = await downloadTask;

            var downloadPath = "D:\\Projekty\\Praca_inz";
            var filePath = "D:\\Projekty\\Praca_inz\\" + dataIndexToDownload + ".zip";

            await download.SaveAsAsync(filePath);

            Unzip(filePath, downloadPath, dataIndexToDownload);

            return "Downloaded successfully";
        }

        private static void Unzip(string filePath, string downloadPath, int dataIndex)
        {

            using (ZipArchive archive = ZipFile.OpenRead(filePath))
            {
                // Assuming there's only one file in the zip archive
                var entry = archive.Entries[0];
                string destinationPath = Path.Combine(downloadPath, dataIndex + ".xml");
                entry.ExtractToFile(destinationPath, true);
            }
        }

        public List<Dictionary<string, string>> ProcessData(int FileIndex)
        {
            var xmlFilePath = "D:\\Projekty\\Praca_inz\\" + FileIndex + ".xml";
            XDocument xDocument = XDocument.Load(xmlFilePath);
            var records = new List<Dictionary<string, string>>();

            var recordElements = xDocument.Descendants("record");
            foreach (var recordElement in recordElements)
            {
                var record = new Dictionary<string, string>();
                var fields = recordElement.Descendants("field");
                foreach (var field in fields)
                {
                    var name = field.Attribute("name").Value;
                    var value = field.Value;
                    record[name] = value;
                }
                records.Add(record);
            }
            records = GetMostRecentData(records);


            return records;
        }

        private List<Dictionary<string, string>> GetMostRecentData(List<Dictionary<string, string>> records)
        {
            string mostRecentYear = "";
            records = GetMostRecentPeriod(records);
            return records;
        }
        private static List<Dictionary<string, string>> GetMostRecentPeriod(List<Dictionary<string, string>> data)
        {
            var mostRecentRecords = new List<Dictionary<string, string>>();
            var countryKey = data[0].Keys.FirstOrDefault(x => x.Contains("Country"));
            var countryGroups = data.GroupBy(d => d[countryKey]).ToList();
            foreach (var group in countryGroups)
            {
                mostRecentRecords.Add(group.OrderByDescending(d => GetMostRecentYear(d)).First());

            }
            return mostRecentRecords;

        }
        public static int GetMostRecentYear(Dictionary<string, string> record)
        {
            if (record.ContainsKey("Year"))
            {
                return int.Parse(record["Year"]);
            }
            if (record.ContainsKey("Period"))
            {
                var period = record["Period"];
                var parts = period.Split('-');
                return int.Parse(parts.Last()); // Get the end year in a "YYYY-YYYY" format
            }
            return 0;
        }
    }
}
