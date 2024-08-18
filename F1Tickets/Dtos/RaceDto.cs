using System.ComponentModel.DataAnnotations;

namespace F1Tickets.Dtos
{
    public class RaceDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Country is required")]
        [DataType(DataType.Text)]
        public string Country { get; set; }
        [Required(ErrorMessage = "Circuit is required")]
        [DataType(DataType.Text)]
        public string Circuit { get; set; }
        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Available Race Day Seats is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value greater than {1}")]
        public int AvailableRaceDaySeats { get; set; }
        [Required(ErrorMessage = "Available Full Weekend Seats is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value greater than {1}")]
        public int AvailableFullWeekendSeats { get; set; }
        [Required(ErrorMessage = "Available F1 PADDOCK CLUB Seats is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value greater than {1}")]
        public int AvailableF1PADDOCKCLUBSeats { get; set; }

        [Required(ErrorMessage = "Photo is required")]
        public IFormFile Photo { get; set; }
        //public string PhotoPath { get; set; }
    }
}
