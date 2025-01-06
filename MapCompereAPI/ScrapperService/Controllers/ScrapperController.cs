using Microsoft.AspNetCore.Mvc;
using ScrapperService.Connectors;
using ScrapperService.Services.UNSDScrapper;
using ScrapperService.Services.WebScrapper;

namespace ScrapperService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScrapperController : ControllerBase
    {
        private DataScrapper _dataScrapper;
        private IDataProcessor _dataProcessor;
        private readonly Scrapper _Scrapper;
        public ScrapperController(IScrapperService UNSDScrapper, ILLMServiceConnector lLMService, IDataProcessor dataProcessor)
        {
            _Scrapper = new Scrapper(lLMService, UNSDScrapper);
            _dataScrapper = new DataScrapper();
            _dataProcessor = dataProcessor;
        }
        [HttpGet("BaseMap")]
        public IActionResult Get()
        {
            try
            {
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("CustomMap")]
        public async Task<IActionResult> GetNewMap([FromQuery] string keyword, [FromQuery] string description)
        {
            var result = await _Scrapper.Scrape(keyword.Replace("_", " "), description.Replace("_", " "));

            try
            {
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("MapFromWeb")]
        public async Task<IActionResult> GetMapFromWeb([FromQuery] string keyword, [FromQuery] string description = "")
        {
            keyword = keyword.Replace("_", " ");
            description = description.Replace("_", " ");
            var mdSourceWebsite = await _dataScrapper.ScrapData(keyword, description);
            var data = await _dataProcessor.ProcessMdData(mdSourceWebsite, keyword+" "+description);
            try
            {
                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
