using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using EmailAPI.Services;
using EmailAPI.Models;

namespace EmailAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EmailController : ControllerBase
	{
		private readonly IEmailContentService _emailContentService;

		public EmailController(IEmailContentService emailContentService)
		{
			_emailContentService = emailContentService;
		}
		[HttpPost]
		public IActionResult SendEmail([FromBody] EmailRequest request)
		{
			if (request == null || string.IsNullOrEmpty(request.OrderNumber))
			{
				return BadRequest("Invalid request");
			}
			var qrCodeUrl = $"http://localhost:5282/api/QRCode/GenerateQRCode?orderNumber={request.OrderNumber}&username={request.Username}&raceCountry={request.RaceCountry}&raceDate={request.RaceDate}&ticketType={request.TicketType}&ticketPrice={request.TicketPrice}";

			var body = _emailContentService.GetEmailBody(
				request.OrderNumber,
				request.Username,
				request.RaceCountry,
				request.RaceDate,
				request.TicketType,
				request.TicketPrice);

			var email = new MimeMessage();
			email.From.Add(new MailboxAddress("F1 Tickets", "THE_EMAIL_SMTP_IS_SENDING_FROM"));
			email.To.Add(MailboxAddress.Parse("YOUEMAIL@email.com_FOR_TESTING/Later can be changed to username email"));
			email.Subject = "Your F1 Ticket is here!";
			email.Body = new TextPart(TextFormat.Html) { Text = body };

			using var smtp = new SmtpClient();
			smtp.Connect("YOUR SMTP SERVER", 587, SecureSocketOptions.StartTls);
			smtp.Authenticate("SMTP_EMAIL", "SMTP_EMAIL_PASSWORD");
			smtp.Send(email);
			smtp.Disconnect(true);

			return Ok();
		}
	}
}
