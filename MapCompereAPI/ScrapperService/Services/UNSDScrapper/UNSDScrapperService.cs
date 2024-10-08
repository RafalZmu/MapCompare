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
        private string _example_data_path = """C:\Users\rafal\Downloads\UNdata_Export_20240624_202226784\UNdata_Export_20240624_202226784.csv""";

        private ILLMServiceConnector _LLMServiceConnector;

        public UNSDScrapperService(ILLMServiceConnector LLMServiceConnector)
        {
            _LLMServiceConnector = LLMServiceConnector;
        }
        public async Task<List<string>> GetDatasetsTitles(string url, HttpClient http)
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

            var downloadPath ="D:\\Projekty\\Praca_inz"; 
            var filePath = "D:\\Projekty\\Praca_inz\\"+dataIndexToDownload+".zip";

            await download.SaveAsAsync(filePath);

            Unzip(filePath, downloadPath, dataIndexToDownload);

            return await linksToDownloadPages[dataIndexToDownload].GetAttributeAsync("href")?? "Not found";
        }

        private static void Unzip(string filePath, string downloadPath, int dataIndex)
        {

            using (ZipArchive archive = ZipFile.OpenRead(filePath))
            {
                // Assuming there's only one file in the zip archive
                var entry = archive.Entries[0];
                string destinationPath = Path.Combine(downloadPath, dataIndex+".xml");
                entry.ExtractToFile(destinationPath, true);
            }
        }

        public Task<Dictionary<string, object>> ProcessData(int FileIndex)
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


            return null;
        }

        private List<Dictionary<string, string>> GetMostRecentData(List<Dictionary<string, string>> records)
        {
            string mostRecentYear = "";
            if(records[0].ContainsKey("Year") == true)
            {
                mostRecentYear = records.Max(x => x["Year"]);
                if(int.Parse(mostRecentYear) > DateTime.Now.Year)
                {
                    mostRecentYear = DateTime.Now.Year.ToString();
                }
                records = records.Where(x => x["Year"] == mostRecentYear).ToList();
            }
            else if(records[0].ContainsKey("Year(s)") == true)
            {
                mostRecentYear = records.Max(x => x["Year(s)"]);
                records = records.Where(x => x["Year(s)"] == mostRecentYear).ToList();
                if(int.Parse(mostRecentYear) > DateTime.Now.Year)
                {
                    mostRecentYear = DateTime.Now.Year.ToString();
                }
            }
            else if (records[0].ContainsKey("Period") == true)
            {
                mostRecentYear = records.Max(x => x["Period"]);
                mostRecentYear = mostRecentYear.Split("-")[1];
                records = records.Where(x => x["Period"].Contains(mostRecentYear)).ToList();
            }
            return records;
        }
    }
}
