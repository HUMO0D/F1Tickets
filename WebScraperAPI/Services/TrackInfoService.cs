using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebScraperAPI.Services
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
			// Construct URL based on trackName
			string url = $"https://www.formula1.com/en/racing/{year}/{trackName}/circuit";
			try
			{
				// Fetch the HTML content from the URL
				var response = await _httpClient.GetStringAsync(url);

				// Load the HTML document
				var htmlDocument = new HtmlDocument();
				htmlDocument.LoadHtml(response);

				// Find the div with the specified class
				var divNode = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'f1-container container pt-l tablet:pt-xl')]");

				if (divNode == null)
				{
					// If the div is not found, return a custom message
					return "<p>Track info not found</p>";
				}
				return divNode.OuterHtml;
			}
			catch (Exception ex)
			{
				// Return a fallback message on error
				return "<p>Unable to retrieve track information</p>";
			}
		}
	}
}
