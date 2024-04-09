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
		public IActionResult GetMap()
		{
			if (System.IO.File.Exists(_mapFilePath))
			{
				// Read the SVG file content
				string svgContent = System.IO.File.ReadAllText(_mapFilePath);

				// Set the response content type
				HttpContext.Response.ContentType = "image/svg+xml";

				// Write the SVG content to the response output stream
				return Content(svgContent);
			}
			else
			{
				// File not found, return a 404 status code or handle it accordingly
				return NotFound();
			}
		}

		[HttpPost]
		public IActionResult PostMap([FromBody] List<CountryDTO> countries)
		{
			string BaseMap = System.IO.File.ReadAllText(_mapFilePath);

			string updatedMap = BaseMap;

			// Loop through the list of countries and update the SVG content
			return Ok();
		}
	}
}