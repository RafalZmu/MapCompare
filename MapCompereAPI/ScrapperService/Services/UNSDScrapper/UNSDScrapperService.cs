﻿using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.Playwright;
using ScrapperService.Connectors;
using ScrapperService.Models;
using System.Globalization;
using System.Linq;

namespace ScrapperService.Services.UNSDScrapper
{
    public class UNSDScrapperService : IScrapperService
    {
        private string _example_data_path = """C:\Users\rafal\Downloads\UNdata_Export_20240624_202226784\UNdata_Export_20240624_202226784.csv""";

        private List<List<string>> _rawData;
        private UNSDFullDataContent _dataContent;
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
            var downloadPath = "D:\\Projekty\\Praca_inz\\DownloadedXML.zip";
            await download.SaveAsAsync(downloadPath);

            return await linksToDownloadPages[dataIndexToDownload].GetAttributeAsync("href")?? "Not found";


        }

        // Ensure the file exists
        public async Task<string> ReadData()
        {
            //Scrap the data from the website

            //Get the data from the file
            _rawData = GetDataFromFile(_example_data_path);

            //Get the data Title and Column names
            _dataContent = GetDataTitleAndColumnNames(_rawData[0], _rawData[1]);

            //Get the all the rows for first country and get the row context

            //Request API for decision whith row is the most relevant to the query

            //Get the data for all countries from the most relevant row

            return _dataContent.ToString();


        }

        public static List<List<string>> GetDataFromFile(string example_data_path)
        {
            if (!File.Exists(example_data_path))
            {
                Console.WriteLine("File not found.");
                throw new ApplicationException("File not found.");
            }
            var rows = new List<List<string>>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
            };

            try
            {
                using (var reader = new StreamReader(example_data_path))
                using (var csv = new CsvReader(reader, config))
                {
                    while (csv.Read())
                    {
                        var row = new List<string>();
                        for (int i = 0; i < csv.ColumnCount; i++)
                        {
                            row.Add(csv.GetField(i));
                        }
                        rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
                throw new ApplicationException("The file could not be read.", ex);
            }

            return rows;
        }

        public static UNSDFullDataContent GetDataTitleAndColumnNames(List<string> firstRow, List<string> secondRow)
        {
            var rowToExtractFrom = firstRow;
            var dataContent = new UNSDFullDataContent();

            if (firstRow.Count == 0 || firstRow.Contains(""))
            {
                Console.WriteLine("Incomplete data in first row.");

                rowToExtractFrom = secondRow;
            }
            foreach (var item in rowToExtractFrom)
            {
                dataContent.ColumnNames.Add(item);
            }

            return dataContent;
        }
    }
}
