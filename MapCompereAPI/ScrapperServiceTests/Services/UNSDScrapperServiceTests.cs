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
        [ExpectedException(typeof(ApplicationException))]
        public void GetDataFromFile_FileNotFound_ThrowsApplicationException()
        {
            // Arrange
            string exampleDataPath = "nonexistentfile.csv";

            // Act
            UNSDScrapperService.GetDataFromFile(exampleDataPath);

            // Assert is handled by ExpectedException
        }
        
        // Test for successful file reading
        [TestMethod]
        public void GetDataFromFile_ValidFile_ReturnsData()
        {
            // Arrange
            string exampleDataPath = "validfile.csv";
            string TestData = """
                "Country or Area","Commodity - Transaction","Year","Unit","Quantity","Quantity Footnotes"
                "Afghanistan","Additives and Oxygenates - Imports","2022","Metric tons,  thousand","0.2","1"
                "Afghanistan","Additives and Oxygenates - Imports","2021","Metric tons,  thousand","0.2","1"
                """;
            // Write a temporary file for testing
            File.WriteAllText(exampleDataPath, TestData);

            // Act
            List<List<string>> result = UNSDScrapperService.GetDataFromFile(exampleDataPath);

            // Assert
            Assert.AreEqual(3, result.Count);
            CollectionAssert.AreEqual(new List<string> { "Country or Area", "Commodity - Transaction", "Year", "Unit", "Quantity", "Quantity Footnotes" }, result[0]);
            CollectionAssert.AreEqual(new List<string> { "Afghanistan","Additives and Oxygenates - Imports","2022","Metric tons,  thousand","0.2","1" }, result[1]);

            // Cleanup
            File.Delete(exampleDataPath);
        }
        [TestMethod]
        public async Task ScrapeSearchResults_ValidQuery_ReturnsData()
        {
            // Arrange
            string Uri = "https://data.un.org/Search.aspx?q=GDP";
            HttpClient client = new();
            IScrapperService scrapper = new UNSDScrapperService(new LLMServiceConnector());

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
            IScrapperService scrapper = new UNSDScrapperService(LLMServiceConnector);

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
            IScrapperService scrapper = new UNSDScrapperService(lLMServiceConnector);

            //Act
            var result = await scrapper.DownloadData(Uri, 3);

            //Assert
            Assert.IsNotNull(result);
            Assert.Equals(result, "Data.aspx?q=Temperature&d=CLINO&f=ElementCode%3a05");
        }
    }
}