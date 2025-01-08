using MapCompereAPI.Connectors;
using MapCompereAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Text.Json;

namespace MapCompereAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MapController : ControllerBase
	{
		private readonly IScrapperConnector _scrapperConnector;

		public MapController(IScrapperConnector scrapperConnector)
		{
			_scrapperConnector = scrapperConnector;

		}

		[HttpGet("GetMap")]
		public async Task<IActionResult> GetMap([FromQuery] string keyword, [FromQuery] string description)
		{
			var data = await _scrapperConnector.ScrapData(keyword, description);
			var dataProcessingService = new DataProcessingService();
			var procesedData = dataProcessingService.CorrectCountryNames(dataProcessingService.JsonToCountryDto(data));
			data = JsonSerializer.Serialize(procesedData, new JsonSerializerOptions { WriteIndented = true});


            return Ok(data);	
		}
		[HttpGet("GetMapFromWeb")]
		public async Task<IActionResult> GetMapFromWeb([FromQuery] string keyword, [FromQuery] string description = null)
		{
			if (string.IsNullOrWhiteSpace(description))
            {
				description = "";
            }
            if (string.IsNullOrWhiteSpace(keyword))
			{
				return Problem("Keyword is required");
            }
            var data = await _scrapperConnector.ScrapDataFromWeb(keyword, description);
			var dataProcessingService = new DataProcessingService();
			var procesedData = dataProcessingService.CorrectCountryNames(dataProcessingService.JsonToCountryDto(data));
			data = JsonSerializer.Serialize(procesedData, new JsonSerializerOptions { WriteIndented = true});


            return Ok(data);	
		}
	}
}