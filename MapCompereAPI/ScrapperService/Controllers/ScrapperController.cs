using Microsoft.AspNetCore.Mvc;
using ScrapperService.Connectors;
using ScrapperService.Services.UNSDScrapper;

namespace ScrapperService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScrapperController : ControllerBase
    {
        private readonly Scrapper _Scrapper;
        public ScrapperController(IScrapperService UNSDScrapper, ILLMServiceConnector lLMService)
        {
            _Scrapper = new Scrapper(lLMService, UNSDScrapper);
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
    }
}
