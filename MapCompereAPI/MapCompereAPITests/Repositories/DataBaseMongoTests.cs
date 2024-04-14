using AutoMapper;
using MapCompereAPI.Mapping;
using MapCompereAPI.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace MapCompereAPI.Repositories.Tests
{
    [TestClass()]
    public class DataBaseMongoTests
    {
        private Mock<ILogger<DataBaseMongo>> _loggerDB;
        private Mock<ILogger<MapService>> _loggerMap;
        private IDocumentDatabase _database;
        private IMapService _mapService;
        private IMapper _mapper;

        [TestInitialize]
        public void Initializer()
        {
            // Arrange
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            }).CreateMapper();
            _loggerDB = new Mock<ILogger<DataBaseMongo>>();
            _loggerMap = new Mock<ILogger<MapService>>();
            _database = new DataBaseMongo(_loggerDB.Object);
            _mapService = new MapService(_database, _loggerMap.Object, _mapper);

        }
        [TestMethod()]
        public async Task GetBaseMap_ShouldReturnMap()
        {
            var map = await _mapService.GetBaseMap();
            Assert.IsNotNull(map);
            Assert.AreEqual("BaseMap", map.Name);
            Assert.IsNotNull(map.SVGImage);
        }

        [TestMethod()]
        public async Task AddMapToDatabase_ShouldAddMapToDatabase()
        {
            var map = new Map()
            {
                Name = "TestMap",
                Description = "TestDescription",
                SVGImage = "TestSVG",
                Creator = "TestCreator",
                CreationDate = DateTime.Now
            };

            var PostResoult = await _mapService.PostMap(map);


            var result = await _mapService.GetMap("TestMap");

            var deleteResoult = await _mapService.DeleteMap("TestMap");

            var resultAfterDelete = await _mapService.GetMap("TestMap");

            Assert.AreEqual(1, PostResoult);
            Assert.AreEqual(1, deleteResoult);

            Assert.IsNotNull(result);
            Assert.AreEqual("TestMap", result.Name);
            Assert.AreEqual("TestDescription", result.Description);
            Assert.AreEqual("TestSVG", result.SVGImage);

            Assert.IsNull(resultAfterDelete);

        }
        [TestMethod()]
        public void TestMappingConfiguration()
        {
            _mapper.ConfigurationProvider.AssertConfigurationIsValid();
            Assert.IsTrue(true);
        }
    }
}