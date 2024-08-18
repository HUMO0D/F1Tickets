using Microsoft.AspNetCore.Mvc;
using WebScraperAPI.Services;
using System.Threading.Tasks;

namespace WebScraperAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TrackInfoController : ControllerBase
	{
		private readonly ITrackInfoService _trackInfoService;

		public TrackInfoController(ITrackInfoService trackInfoService)
		{
			_trackInfoService = trackInfoService;
		}

		[HttpGet]
		public async Task<IActionResult> GetTrackInfo([FromQuery] string trackName, string year)
		{
			if (string.IsNullOrEmpty(trackName) || string.IsNullOrEmpty(year))
			{
				return BadRequest("Track info Not Found.");
			}

			var trackInfo = await _trackInfoService.GetTrackInfoAsync(trackName, year);
			if (string.IsNullOrEmpty(trackInfo) || string.IsNullOrEmpty(trackName))
			{
				return NotFound("Track info not found.");
			}

			return Content(trackInfo, "text/html");
		}
	}
}
