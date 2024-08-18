using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace F1Tickets.Services
{
    public class F1ResultsService
    {
        private readonly HttpClient _httpClient;

        public F1ResultsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<JObject> GetRaceResultsAsync(int year, int round)
        {
            var url = $"http://ergast.com/api/f1/{year}/{round}/results.json";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JObject.Parse(json);
        }

        public async Task<JObject> GetFastestLapAsync(int year, int round, int fastest)
        {
            var url = $"http://ergast.com/api/f1/{year}/{round}/fastest/{fastest}/results.json";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JObject.Parse(json);
        }
    }
}
