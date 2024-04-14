using Microsoft.AspNetCore.Mvc;

namespace MapCompereAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MapController : ControllerBase
	{
		private readonly string _mapFilePath = "./Assets/WorldMap.svg";
		private readonly IMapService _mapService; 

		public MapController(IMapService mapService)
		{
			_mapService = mapService;
		}

		[HttpGet]
		public async Task<IActionResult> GetBaseMap()
		{
			var map = await _mapService.GetBaseMap();
			if(map == null)
			{
				return NotFound();
			}
			return Ok(map);
		}

		[HttpPost]
		public async Task<IActionResult> PostMap([FromBody] List<CountryDTO> countries)
		{
			string BaseMap = System.IO.File.ReadAllText(_mapFilePath);

			string updatedMap = BaseMap;

			// Loop through the list of countries and update the SVG content
			return Ok();
		}

		[HttpDelete]
		public async Task<IActionResult> DeleteMap(string mapName)
		{
            _mapService.DeleteMap(mapName);
            return Ok();
        }
	}
}