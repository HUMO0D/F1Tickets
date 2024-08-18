using Newtonsoft.Json;
using System.Text;

namespace F1Tickets.Services
{
	public interface ITrackInfoService
	{
		Task<string> GetTrackInfoAsync(string trackName, string year);
	}
	public class TrackInfoService : ITrackInfoService
	{
		private readonly HttpClient _httpClient;

		public TrackInfoService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<string> GetTrackInfoAsync(string trackName, string year)
		{
			var requestUri = $"http://localhost:5252/api/trackinfo?trackName={trackName}&year={year}";
			var response = await _httpClient.GetStringAsync(requestUri);
			return response;
		}
	}
}
