using F1Tickets.Data;
using F1Tickets.Models;
using F1Tickets.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace F1Tickets.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
		private readonly AppDbContext _context;
		private readonly ITrackInfoService _trackInfoService;
		public TicketsController(AppDbContext context, ITrackInfoService trackInfoService)
		{
			_context = context;
			_trackInfoService = trackInfoService;
		}
		public async Task<IActionResult> TicketsView(int id)
		{
			var race = await _context.Race.FirstOrDefaultAsync(x => x.Id == id);
			var ticketTypes = await _context.TicketType.ToListAsync();
		    ViewBag.TicketTypes = ticketTypes;

			// Fetch track information
			var trackname = race.Country;
			var tracknamelower = trackname.ToLower();
			var track = tracknamelower.Replace(" ", "-");
			var year = race.Date.Year.ToString();
			//Console.Write(year);
			//Console.Write(track);
			try
			{
				var trackInfoHtml = await _trackInfoService.GetTrackInfoAsync(track, year);
				ViewBag.TrackInfoHtml = trackInfoHtml;
			}
			catch (Exception ex)
			{
				ViewBag.TrackInfoHtml = "<p>Track info not found</p>";
			}

			return View(race);
		}
    }
}
