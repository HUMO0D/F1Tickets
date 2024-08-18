using F1Tickets.Models;
using F1Tickets.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace F1Tickets.Controllers
{
    public class F1ResultsController : Controller
    {
        private readonly F1ResultsService _f1ResultsService;

        public F1ResultsController(F1ResultsService f1ResultsService)
        {
            _f1ResultsService = f1ResultsService;
        }

        [HttpGet]
        public IActionResult F1ResultsView()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetRaceResults(int year, int round)
        {
            var raceResults = await _f1ResultsService.GetRaceResultsAsync(year, round);

            var raceData = raceResults["MRData"]["RaceTable"]["Races"][0];
            var raceName = raceData["raceName"].ToString();
            var circuitName = raceData["Circuit"]["circuitName"].ToString();
            var raceDate = raceData["date"].ToString();

            var results = raceData["Results"];
            var resultList = new List<RaceResultViewModel>();

            foreach (var result in results)
            {
                resultList.Add(new RaceResultViewModel
                {
                    Position = result["positionText"].ToString(),
                    Driver = result["Driver"]["givenName"].ToString() + " " + result["Driver"]["familyName"].ToString(),
                    Constructor = result["Constructor"]["name"].ToString(),
                    Time = result["Time"]?["time"]?.ToString() ?? " ",
                    Points = result["points"].ToString()
                });
            }

            var viewModel = new RaceResultsViewModel
            {
                RaceName = raceName,
                CircuitName = circuitName,
                RaceDate = raceDate,
                Results = resultList
            };
            return Json(new { success = true, data = viewModel });
        }

        [HttpPost]
        public async Task<IActionResult> GetFastestLap(int year, int round, int fastest)
        {
            var fastestLap = await _f1ResultsService.GetFastestLapAsync(year, round, fastest);

            var raceData = fastestLap["MRData"]["RaceTable"]["Races"][0];
            var raceName = raceData["raceName"].ToString();
            var circuitName = raceData["Circuit"]["circuitName"].ToString();
            var driverName = raceData["Results"][0]["Driver"]["givenName"].ToString() + " " + raceData["Results"][0]["Driver"]["familyName"].ToString();
            var fastestLapTime = raceData["Results"][0]["FastestLap"]["Time"]["time"].ToString();

            var viewModel = new FastestLapViewModel
            {
                RaceName = raceName,
                CircuitName = circuitName,
                DriverName = driverName,
                FastestLapTime = fastestLapTime
            };
            return Json(new { success = true, data = viewModel });
        }
    }
}
