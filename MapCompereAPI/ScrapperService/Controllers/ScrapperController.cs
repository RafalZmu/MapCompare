using Microsoft.AspNetCore.Mvc;
using ScrapperService.Services;

namespace ScrapperService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScrapperController : ControllerBase
    {
        private readonly IScrapperService _UNSDScrapper;
        public ScrapperController(IScrapperService UNSDScrapper)
        {
            _UNSDScrapper = UNSDScrapper;

        }
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_UNSDScrapper.ReadData());
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
