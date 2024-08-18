using F1Tickets.Areas.Identity.Data;
using F1Tickets.Data;
using F1Tickets.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace F1Tickets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser>_userManager;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, AppDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()

		{
			//var race = await _context.Race.ToListAsync();
			var race = await _context.Race
        	    .OrderBy(r => r.Date)
	            .ToListAsync();
			return View(race);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
