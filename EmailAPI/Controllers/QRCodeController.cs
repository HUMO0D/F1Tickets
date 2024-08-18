using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace EmailAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRCodeController : ControllerBase
    {
        private byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        [HttpGet("GenerateQRCode")]
		public async Task<ActionResult> GenerateQRCode(string orderNumber, string username, string raceCountry, string raceDate, string ticketType, string ticketPrice)
		{
			if (string.IsNullOrEmpty(orderNumber) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(raceCountry) || string.IsNullOrEmpty(raceDate) || string.IsNullOrEmpty(ticketType) || string.IsNullOrEmpty(ticketPrice))
			{
				return BadRequest("QRCodeText is required.");
            }

			var qrCodeText = $"Order Number: {orderNumber}\n" +
							 $"Username: {username}\n" +
							 $"Race Country: {raceCountry}\n" +
							 $"Race Date: {raceDate}\n" +
							 $"Ticket Type: {ticketType}\n" +
							 $"Ticket Price: {ticketPrice}";
           
			var qrCodeGenerator = new QRCodeGenerator();
			var qrCodeData = qrCodeGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q);

			using (var qrCode = new QRCode(qrCodeData))
            using (var qrCodeImage = qrCode.GetGraphic(20))
            {
                var bytes = ImageToByteArray(qrCodeImage);
                return File(bytes, "image/jpeg");
            }
        }
    }
}
