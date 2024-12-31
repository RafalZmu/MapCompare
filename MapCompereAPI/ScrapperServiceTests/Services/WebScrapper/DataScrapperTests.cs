using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void ScrapperMdProcessingTest()
        {
            //Arrange
            string mdString = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + $"\\Assets\\ScrapperMdExample.json");
            var processedData = DataProcessor.ProcessMdData(mdString);
        }
    }
}