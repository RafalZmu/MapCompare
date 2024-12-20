using MapCompereAPI.Connectors;
using Microsoft.AspNetCore.Mvc;

namespace MapCompereAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MapController : ControllerBase
	{
		private readonly string _mapFilePath = "./Assets/WorldMap.svg";
		private readonly IScrapperConnector _scrapperConnector;

		public MapController(IScrapperConnector scrapperConnector)
		{
			_scrapperConnector = scrapperConnector;

		}

		[HttpGet("GetMap")]
		public async Task<IActionResult> GetMap([FromQuery] string keyword, [FromQuery] string description)
		{
			var data = await _scrapperConnector.ScrapData(keyword, description);


			return Ok(data);	
		}



		[HttpPost]
		public async Task<IActionResult> PostMap([FromBody] List<CountryDTO> countries)
		{
			string BaseMap = System.IO.File.ReadAllText(_mapFilePath);

			string updatedMap = BaseMap;

			// Loop through the list of countries and update the SVG content
			return Ok();
		}
	}
}