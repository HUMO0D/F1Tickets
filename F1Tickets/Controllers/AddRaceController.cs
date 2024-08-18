using Microsoft.AspNetCore.Mvc;
using F1Tickets.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using F1Tickets.Data;
using F1Tickets.Dtos;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;


namespace F1Tickets.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AddRaceController : Controller
    {
        private readonly AppDbContext _context;

        public AddRaceController(AppDbContext context) {
        _context = context;
        }

        public IActionResult AddRaceView()
        {
            return View();
        }


		[HttpPost]
		public async Task<IActionResult> Create(RaceDto race)
		{
			if (ModelState.IsValid)
			{
				if (race.Photo != null && race.Photo.Length > 0)
				{
					var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
					var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(race.Photo.FileName);
					var filePath = Path.Combine(uploadsFolder, uniqueFileName);

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await race.Photo.CopyToAsync(stream);
					}

					var Data = new Race();
					Data.PhotoPath = "/uploads/" + uniqueFileName;

					Data.Country = race.Country;
					Data.Circuit = race.Circuit;
					Data.Date = race.Date;
					Data.AvailableRaceDaySeats = race.AvailableRaceDaySeats;
					Data.AvailableFullWeekendSeats = race.AvailableFullWeekendSeats;
					Data.AvailableF1PADDOCKCLUBSeats = race.AvailableF1PADDOCKCLUBSeats;

					_context.Race.Add(Data);
					await _context.SaveChangesAsync();
					TempData["SuccessMessage"] = "Race Added Successfully";
					return RedirectToAction("Index", "Home");
				}
			}
			TempData["FaildMessage"] = "Race Not Added";
			return RedirectToAction("AddRaceView", "AddRace");
		}



		[HttpPost]
		public async Task<IActionResult> DeleteRace(int id)
		{
			var race = await _context.Race.FirstOrDefaultAsync(x => x.Id == id);
			if (race == null)
			{
				TempData["FaildMessage"] = "No Race to delete.";
				return RedirectToAction("Index", "Home");
			}
			var orders = _context.Order.Where(order => order.RaceId == id);
			if (orders.Any())
			{
				_context.Order.RemoveRange(orders);
			}
			_context.Race.Remove(race);
			await _context.SaveChangesAsync();
			TempData["SuccessMessage"] = "Race Deleted Successfully";
			return RedirectToAction("Index", "Home");
		}
	}
}
