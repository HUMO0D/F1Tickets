using System.IO;
using System.Text;

namespace EmailAPI.Services
{
	public interface IEmailContentService
	{
		string GetEmailBody(string orderNumber, string username, string raceCountry, string raceDate, string ticketType, string ticketPrice);
		string GetQRCodeUrl(string orderNumber, string username, string raceCountry, string raceDate, string ticketType, string ticketPrice);

	}
	public class EmailContentService : IEmailContentService
	{
		private readonly string _filePath;
		private readonly string _qrCodeBaseUrl;
		public EmailContentService(IWebHostEnvironment env)
		{
			_filePath = Path.Combine(env.ContentRootPath, "EmailBody", "EmailBody.cshtml");
			_qrCodeBaseUrl = "http://localhost:5282/api/QRCode/GenerateQRCode";

		}
		public string GetEmailBody(string orderNumber, string username, string raceCountry, string raceDate, string ticketType, string ticketPrice)
		{
			string emailBody = File.ReadAllText(_filePath, Encoding.UTF8);
			emailBody = emailBody.Replace("{{OrderNumber}}", orderNumber);
			emailBody = emailBody.Replace("{{Username}}", username);
			emailBody = emailBody.Replace("{{RaceCountry}}", raceCountry);
			emailBody = emailBody.Replace("{{RaceDate}}", raceDate);
			emailBody = emailBody.Replace("{{TicketType}}", ticketType);
			emailBody = emailBody.Replace("{{TicketPrice}}", ticketPrice);

			//For testing the email should be viewd at the same host machine as the website
			var qrCodeUrl = GetQRCodeUrl(orderNumber, username, raceCountry, raceDate, ticketType, ticketPrice);
			emailBody = emailBody.Replace("{{QRCodeUrl}}", qrCodeUrl);
			
			//emailBody += $"<br><img src='{qrCodeUrl}' alt='QR Code' />";

			return emailBody;
		}
		public string GetQRCodeUrl(string orderNumber, string username, string raceCountry, string raceDate, string ticketType, string ticketPrice)
		{
			return $"{_qrCodeBaseUrl}?orderNumber={Uri.EscapeDataString(orderNumber)}&username={Uri.EscapeDataString(username)}&raceCountry={Uri.EscapeDataString(raceCountry)}&raceDate={Uri.EscapeDataString(raceDate)}&ticketType={Uri.EscapeDataString(ticketType)}&ticketPrice={Uri.EscapeDataString(ticketPrice)}";
		}
	}
}
