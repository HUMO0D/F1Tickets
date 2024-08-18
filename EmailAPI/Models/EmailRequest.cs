namespace EmailAPI.Models
{
	public class EmailRequest
	{
		public string OrderNumber { get; set; }
		public string Username { get; set; }
		public string RaceCountry { get; set; }
		public string RaceDate { get; set; }
		public string TicketType { get; set; }
		public string TicketPrice { get; set; }
	}
}
