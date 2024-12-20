using MapCompereAPI.Services;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScrapperService.Connectors;
using ScrapperService.Services.UNSDScrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapperService.Services.Tests
{
    [TestClass()]
    public class UNSDScrapperServiceTests
    {
        [TestMethod]
        public async Task ScrapeSearchResults_ValidQuery_ReturnsData()
        {
            // Arrange
            string Uri = "https://data.un.org/Search.aspx?q=GDP";
            HttpClient client = new();
            IScrapperService scrapper = new UNSDScrapperService();

            // Act
            IEnumerable<string> result = await scrapper.GetDatasetsTitles(Uri, client);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() > 0);
        }

        [TestMethod]
        public async Task GetPrediction()
        {
            //Arrange
            string Uri = "https://data.un.org/Search.aspx?q=Temperature";
            HttpClient client = new();
            ILLMServiceConnector LLMServiceConnector = new LLMServiceConnector();
            IScrapperService scrapper = new UNSDScrapperService();

            IEnumerable<string> titles = await scrapper.GetDatasetsTitles(Uri, client);
            string titlesString = "";
            int index = 0;
            foreach (string title in titles)
            {
                titlesString += index.ToString() + "-"+ title+"//";
                index++;
            }

            //Act 
            string result = await LLMServiceConnector.GetPrediction(titlesString, "Which one of provided Titles matches best to this search: Bulb Temperature minimum. Return only the number that corresponst to the most relevant title. Don't provide any more information, return only the number without any other text, The titles are separated with //.");
            result = result.Trim();
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length == 1);
        }

        [TestMethod]
        public async Task DownloadingTheFile()
        {
            //Arrange
            string Uri = "https://data.un.org/Search.aspx?q=Temperature";
            ILLMServiceConnector lLMServiceConnector = new LLMServiceConnector();
            IScrapperService scrapper = new UNSDScrapperService();

            //Act
            var result = await scrapper.DownloadData(Uri, 3);

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task ProcessData()
        {
            //Arrange
            ILLMServiceConnector lLMServiceConnector = new LLMServiceConnector();
            IScrapperService scrapper = new UNSDScrapperService();

            //Act
            var result = scrapper.ProcessData(1);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public async Task GetPredictionBestKeys()
        {
            //Arrange
            ILLMServiceConnector LLMServiceConnector = new LLMServiceConnector();
            IScrapperService scrapper = new UNSDScrapperService();

            //Act
            var data = scrapper.ProcessData(3);
            var Data = data[0];
            var keysString = "";
            foreach (var key in Data)
            {
                if (key.Key == "Country or territory" || key.Key == "Year" || key.Key   == "Year(s)" || key.Key == "Period")
                {
                    continue;
                }
                keysString += "|"+key.Key+","+key.Value+"|";
            }
            keysString = "["+ keysString + "]";
            var query = "Anual GDP";
            var mostRelevantKeys = await LLMServiceConnector.GetPrediction(keysString, $"In data part you will be provided comma separeted list of keys and corresponding values to some data dictionary. The data is associated with this query={query}. You need to return only two best matching keys. Your response should exactly match the template: [Description=The key with highest possibility of containing the description of the informations in the dataset must be string, Values=The key with highest possibility of containing the numerical values of the dataset]. Ignore the keys with country or year in them. The keys and values are listed in the squere brackets");

            Assert.IsNotNull( mostRelevantKeys );
            Assert.IsTrue(mostRelevantKeys.Contains("Description"));
            Assert.IsTrue(mostRelevantKeys.Contains("Values"));

        }

        [TestMethod]
        public void GetAllDescriptions()
        {
            //Arrange
            ILLMServiceConnector LLMServiceConnector = new LLMServiceConnector();
            IScrapperService scrapper = new UNSDScrapperService();
            var data = scrapper.ProcessData(3);

            //Act
            List<string> recordsDescriptions = Scrapper.GetAllRecordDescriptions(data, "Item");

            //Assert
            Assert.IsNotNull(recordsDescriptions);
        }

        [TestMethod]
        public void RemoveUnwantedKeys()
        {
            //Arrange
            ILLMServiceConnector LLMServiceConnector = new LLMServiceConnector();
            IScrapperService scrapper = new UNSDScrapperService();
            var data = scrapper.ProcessData(3);
            var mostRelevantKeys = "Item,Value";

            //Act
            var result = Scrapper.RemoveUnwantedKeys(data, mostRelevantKeys);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public async Task GetMostRelevantDescription()
        {
            //Arrange
            ILLMServiceConnector LLMServiceConnector = new LLMServiceConnector();
            IScrapperService scrapper = new UNSDScrapperService();
            var data = scrapper.ProcessData(3);
            List<string> recordsDescriptions = Scrapper.GetAllRecordDescriptions(data, "Item");
            var query = "Anual GDP";

            //Act
            var mostRelevantDescription = await LLMServiceConnector.GetPrediction(string.Join(",", recordsDescriptions), $"Which one of provided descriptions matches best to this query: {query}. Return only the number that corresponst to the most relevant query");

            //Assert
            Assert.IsNotNull(mostRelevantDescription);


        }

        [TestMethod]
        public async Task IntegrationTest()
        {
            //Arrange
            Scrapper scrapper = new(new LLMServiceConnector(), new UNSDScrapperService());
            string Uri = "https://data.un.org/Search.aspx?q=Temperature";
            string query = "Annual average temperature";

            //Act
            var result = await scrapper.Scrape(Uri, query);

            //Assert
            Assert.IsNotNull(result);

        }
        [TestMethod]
        public async Task ExtractCountrySynonymsFromJson()
        {
            //Arrange
            var dataProcessingService = new DataProcessingService();

            //Act
            (var correctCountries, var synonyms) = dataProcessingService.GetDataFromJSON();


            //Assert
            Assert.IsNotNull(correctCountries);
            Assert.IsNotNull(synonyms);
            Assert.IsTrue(correctCountries.ContainsKey(synonyms["pl"]));
        }
    }
}