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
            DataScrapper scrapper = new DataScrapper();
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

            //Act
            var processedData = await dataProcessor.ProcessMdData(mdString, "current gdp");

            //Assert
            Assert.IsNotNull(processedData);
            Assert.IsTrue(processedData.Length > 100);
        }

        [TestMethod()]
        public async Task ScrapperE2E()
        {
            // Arrange
            string keyword = "average Temperature";
            string description = "";
            var dataProcessor = new DataProcessor(new LLMServiceConnector());
            DataScrapper scrapper = new DataScrapper();

            // Act
            string result = await scrapper.ScrapData(keyword, description);
            string processedData = await dataProcessor.ProcessMdData(result, keyword + " " + description);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);

        }
    }
}