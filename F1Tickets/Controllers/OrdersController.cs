using F1Tickets.Data;
using F1Tickets.Dtos;
using F1Tickets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

//
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq.Dynamic;

namespace F1Tickets.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
		private readonly AppDbContext _context;
		private readonly AuthDbContext _Authcontext;
		private readonly IEmailService _emailService;
		public OrdersController(AppDbContext context, AuthDbContext Authcontext, IEmailService emailService)
		{
			_context = context;
			_Authcontext = Authcontext;
			_emailService = emailService;
		}


		public async Task<IActionResult> OrdersView()
		{
			var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(UserId))
			{
				TempData["FaildMessage"] = "Please Sign In to order";
				return RedirectToAction("Index", "Home");
			}

			//var orders = await _context.Order
			//	.Where(order => order.UserId == UserId)
			//	.ToListAsync();
			//ViewBag.Orders = orders;


			//var MyOrders =new List<dynamic>();
			//foreach (var order in orders)
			//{
			//	var race = await _context.Race.FirstOrDefaultAsync(x => x.Id == order.RaceId);
			//	var ticket = await _context.TicketType.Where(x => x.Id == order.TicketTypeId).FirstOrDefaultAsync();
			//	if (race != null && ticket != null)
			//	{
			//		MyOrders.Add(new
			//		{
			//			OrderNumber = "F-" + order.Id,
			//			OrderRace = race.Country,
			//			OrderDate = race.Date.ToString("dd/MM/yyyy"),
			//			TicketType = ticket.Type,
			//			TicketPrice = ticket.Price.ToString("F0"),
			//		});
			//	}				
			//}
			//ViewBag.MyOrders = MyOrders;
			return View();
		}

		public async Task<JsonResult> GetOrderFromDB()
		{
			//try
			//{
				var draw = Request.Form["draw"].FirstOrDefault();
				var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
				var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
				var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
				var searchValue = Request.Form["search[value]"].FirstOrDefault();
				int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "10");
				int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

				var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

				// Query database directly
				var query = from order in _context.Order
							join race in _context.Race on order.RaceId equals race.Id
							join ticket in _context.TicketType on order.TicketTypeId equals ticket.Id
							where order.UserId == UserId
							select new
							{
								OrderNumber = "F-" + order.Id,
								OrderRace = race.Country,
								OrderDate = race.Date,
								TicketType = ticket.Type,
								TicketPrice = ticket.Price
							};

				// Retrieve data without filtering
				var dataList = await query.ToListAsync();

				// Apply search filter in memory
				if (!string.IsNullOrEmpty(searchValue))
				{
					searchValue = searchValue.ToLower();
					dataList = dataList
						.Where(x =>
							x.OrderNumber.ToLower().Contains(searchValue) ||
							x.OrderRace.ToLower().Contains(searchValue) ||
							x.OrderDate.ToString("dd/MM/yyyy").ToLower().Contains(searchValue) ||
							x.TicketType.ToLower().Contains(searchValue) ||
							x.TicketPrice.ToString("F0").ToLower().Contains(searchValue))
						.ToList();
				}

				//// Apply sorting in memory
				//if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
				//{
				//	switch (sortColumn)
				//	{
				//		case "OrderNumber":
				//			Console.Write("SortingOrderNumber...");
				//			dataList = sortColumnDirection == "asc"
				//				? dataList.OrderBy(x => x.OrderNumber).ToList()
				//				: dataList.OrderByDescending(x => x.OrderNumber).ToList();
				//			break;
				//		case "OrderRace":
				//			dataList = sortColumnDirection == "asc"
				//				? dataList.OrderBy(x => x.OrderRace).ToList()
				//				: dataList.OrderByDescending(x => x.OrderRace).ToList();
				//			break;
				//		case "OrderDate":
				//			dataList = sortColumnDirection == "asc"
				//				? dataList.OrderBy(x => x.OrderDate).ToList()
				//				: dataList.OrderByDescending(x => x.OrderDate).ToList();
				//			break;
				//		case "TicketType":
				//			dataList = sortColumnDirection == "asc"
				//				? dataList.OrderBy(x => x.TicketType).ToList()
				//				: dataList.OrderByDescending(x => x.TicketType).ToList();
				//			break;
				//		case "TicketPrice":
				//			dataList = sortColumnDirection == "asc"
				//				? dataList.OrderBy(x => x.TicketPrice).ToList()
				//				: dataList.OrderByDescending(x => x.TicketPrice).ToList();
				//			break;
				//		default:
				//			dataList = dataList.OrderBy(x => x.OrderNumber).ToList(); // Default sorting
				//			break;
				//	}
				//}

				// Apply pagination in memory
				var paginatedData = dataList.Skip(skip).Take(pageSize).ToList();

				// Total record count before filtering
				var totalRecord = await _context.Order
					.Where(order => order.UserId == UserId)
					.CountAsync();

				var returnObj = new
				{
					draw = draw,
					recordsTotal = totalRecord,
					recordsFiltered = dataList.Count,
					data = paginatedData.Select(x => new
					{
						x.OrderNumber,
						x.OrderRace,
						OrderDate = x.OrderDate.ToString("dd/MM/yyyy"),
						x.TicketType,
						TicketPrice = x.TicketPrice.ToString("F0")
					}).ToList()
				};

				return Json(returnObj);
			//}
			//catch (Exception ex)
			//{
			//	Console.Write("ORDERERROR: ");
			//	Console.Write(ex.Message);
			//	return Json(new { error = ex.Message });
			//}
		}
		

		//////////////////////////////////////////////////////



		[HttpPost]
        public async Task<IActionResult> CreateOrder(int RaceId, int TicketTypeId)
        {
			if (!User.Identity.IsAuthenticated)
			{
				TempData["FaildMessage"] = "Please Sign In to order.";
				return RedirectToAction("Index", "Home");
			}
			if (ModelState.IsValid)
            {
				var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

				if (string.IsNullOrEmpty(UserId))
				{
					TempData["FaildMessage"] = "Please Sign In to order";
					return RedirectToAction("Index", "Home");
				}
				//    Create Order
				var order = new Order
				{
					UserId = UserId,
					TicketTypeId = TicketTypeId,
					RaceId = RaceId
				};


				//   Decrease Available Seats from the Race Table:

				// Retrieve TicketType name
				var ticketType = await _context.TicketType
					.Where(tt => tt.Id == TicketTypeId)
					.Select(tt => tt.Type)
					.FirstOrDefaultAsync();

				if (ticketType == null)
				{
					TempData["FaildMessage"] = "TicketType not found.";
					return RedirectToAction("Index", "Home");
				}

				// Retrieve Race and check available seats
				var race = await _context.Race
					.Where(r => r.Id == RaceId)
					.FirstOrDefaultAsync();

				if (race == null)
				{
					TempData["FaildMessage"] = "Race not found.";
					return RedirectToAction("Index", "Home");
				}

				//Retrive the UserID to get the username for the email
				var user = await _Authcontext.Users
					.FirstOrDefaultAsync(x => x.Id == UserId);
				if (user == null)
				{
					TempData["FaildMessage"] = "User's table not found.";
					return RedirectToAction("Index", "Home");
				}
				if (user.UserName == null)
				{
					TempData["FaildMessage"] = "User ordered not found.";
					return RedirectToAction("Index", "Home");
				}


				int availableSeats = 0;

				// Determine available seats based on TicketType
				switch (ticketType)
				{
					case "Race Day":
						availableSeats = race.AvailableRaceDaySeats;
						break;
					case "Full Weekend":
						availableSeats = race.AvailableFullWeekendSeats;
						break;
					case "F1 PADDOCK CLUB":
						availableSeats = race.AvailableF1PADDOCKCLUBSeats;
						break;
					default:
						TempData["FaildMessage"] = "Invalid TicketType.";
						return RedirectToAction("Index", "Home");
				}

				if (availableSeats <= 0)
				{
					TempData["FaildMessage"] = "No available seats.";
					return RedirectToAction("Index", "Home");
				}

				// Update the available seats
				switch (ticketType)
				{
					case "Race Day":
						race.AvailableRaceDaySeats -= 1;
						break;
					case "Full Weekend":
						race.AvailableFullWeekendSeats -= 1;
						break;
					case "F1 PADDOCK CLUB":
						race.AvailableF1PADDOCKCLUBSeats -= 1;
						break;
				}

				_context.Order.Add(order);
                await _context.SaveChangesAsync();

				// Prepare the email content
				var orderNumber = "F-" + order.Id;
				var email = User.FindFirstValue(ClaimTypes.Name);
				var username = email?.Split('@').FirstOrDefault();
				if (username == null) 
				{
					TempData["FaildMessage"] = "No User ordered found.";
					return RedirectToAction("Index", "Home");
				}
				var raceCountry = race.Country;
				var raceDate = race.Date.ToString("dd/MM/yyyy");
				var ticketPriceint = await _context.TicketType
					.Where(tt => tt.Id == TicketTypeId)
					.Select(tt => tt.Price)
					.FirstOrDefaultAsync();
				string ticketPrice = ticketPriceint.ToString();


				// Send email
				await _emailService.SendOrderNotificationAsync(orderNumber, username, raceCountry, raceDate, ticketType, ticketPrice);

				TempData["SuccessMessage"] = "Order Placed Successfully";
                return RedirectToAction("OrdersView", "Orders");
            }
            TempData["FaildMessage"] = "Order Not Added";
            return RedirectToAction("Index", "Home");
        }




		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> ManageOrdersView()
        {
            var orders = await _context.Order.ToListAsync();
            ViewBag.Orders = orders;

            var AllOrders = new List<dynamic>();
            foreach (var order in orders)
            {
                var race = await _context.Race.FirstOrDefaultAsync(x => x.Id == order.RaceId);
                var ticket = await _context.TicketType.Where(x => x.Id == order.TicketTypeId).FirstOrDefaultAsync();
				var user = await _Authcontext.Users.FirstOrDefaultAsync(x => x.Id == order.UserId);
				if (user == null)
				{
					TempData["FaildMessage"] = "There is somthing wrong with the records, Some Orders have no User Names";
					return RedirectToAction("Index", "Home");
				}

				if (race != null && ticket != null)
                {
                    AllOrders.Add(new
                    {
						Username = user.UserName.Split('@')[0],
						OrderId = order.Id,
                        OrderRace = race.Country,
                        OrderDate = race.Date.ToString("dd/MM/yyyy"),
                        TicketType = ticket.Type,
                        TicketPrice = ticket.Price.ToString("F0"),
                    });
                }
            }
            ViewBag.AllOrders = AllOrders;
            return View();
        }




		[HttpPost]
		public async Task<IActionResult> DeleteOrder(int OrderId)
		{
			if (!User.Identity.IsAuthenticated && !User.IsInRole("Admin"))
			{
				TempData["FaildMessage"] = "You have to be an Admin to delete.";
				return RedirectToAction("Index", "Home");
			}
			var order = await _context.Order.FirstOrDefaultAsync(x => x.Id == OrderId);
			if (order == null)
			{
				TempData["FaildMessage"] = "No order to delete.";
				return RedirectToAction("ManageOrdersView", "Orders");
			}

			//Add Seat
			var ticketType = await _context.TicketType.FirstOrDefaultAsync(x => x.Id == order.TicketTypeId);
			var race = await _context.Race.Where(r => r.Id == order.RaceId).FirstOrDefaultAsync();
			if (race == null) {
				TempData["FaildMessage"] = "Something is wrong, No Race associated to the Order.";
				return RedirectToAction("ManageOrdersView", "Orders");
			}
			if (ticketType == null)
			{
				TempData["FaildMessage"] = "Something is wrong, No Ticket Type associated to the Order.";
				return RedirectToAction("ManageOrdersView", "Orders");
			}
			switch (ticketType.Type)
			{
				case "Race Day":
					race.AvailableRaceDaySeats += 1;
					break;
				case "Full Weekend":
					race.AvailableFullWeekendSeats += 1;
					break;
				case "F1 PADDOCK CLUB":
					race.AvailableF1PADDOCKCLUBSeats += 1;
					break;
			}

			_context.Order.Remove(order);
			await _context.SaveChangesAsync();
			TempData["SuccessMessage"] = "Order Deleted Successfully";
			return RedirectToAction("ManageOrdersView", "Orders");
		}




	}
}
