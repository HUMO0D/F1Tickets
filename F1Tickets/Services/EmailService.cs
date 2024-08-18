using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public interface IEmailService
{
	Task SendOrderNotificationAsync(string orderNumber, string username, string raceCountry, string raceDate, string ticketType, string ticketPrice);
}

public class EmailService : IEmailService
{
	private readonly HttpClient _httpClient;

	public EmailService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task SendOrderNotificationAsync(string orderNumber, string username, string raceCountry, string raceDate, string ticketType, string ticketPrice)
	{
		var emailRequest = new EmailRequest
		{
			OrderNumber = orderNumber,
			Username = username,
			RaceCountry = raceCountry,
			RaceDate = raceDate,
			TicketType = ticketType,
			TicketPrice = ticketPrice
		};
		var requestContent = new StringContent(
			JsonConvert.SerializeObject(emailRequest),
			Encoding.UTF8,
			"application/json");

		var response = await _httpClient.PostAsync("http://localhost:5282/api/Email", requestContent);
		response.EnsureSuccessStatusCode();

	}
}
