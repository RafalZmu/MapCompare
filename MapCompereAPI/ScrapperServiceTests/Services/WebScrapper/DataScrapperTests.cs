using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScrapperService.Connectors;
using ScrapperService.Services.WebScrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapperService.Services.WebScrapper.Tests
{
    [TestClass()]
    public class DataScrapperTests
    {
        [TestMethod()]
        public async Task ScrapData_ValidQuery_ReturnsData()
        {
            // Arrange
            string keyword = "GDP";
            string description = "GDP";
            var dataProcessor = new DataProcessor(new LLMServiceConnector());
            DataScrapper scrapper = new DataScrapper(dataProcessor);
            // Act
            string result = await scrapper.ScrapData(keyword, description);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }

        [TestMethod()]
        public async Task ScrapperMdProcessingTest()
        {
            //Arrange
            var LLMServiceConnector = new LLMServiceConnector();
            var dataProcessor = new DataProcessor(LLMServiceConnector);
            string mdString = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + $"\\Assets\\ScrapperMdExample.json");
            var processedData = await dataProcessor.ProcessMdData(mdString, "current gdp");
        }
    }
}