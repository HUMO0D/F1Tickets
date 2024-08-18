using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Tickets.Models
{
    public class Race
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Country { get; set; }
        public string Circuit { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public int AvailableRaceDaySeats { get; set; }
        public int AvailableFullWeekendSeats { get; set; }
        public int AvailableF1PADDOCKCLUBSeats { get; set; }
        public string PhotoPath { get; set; }

    }
}
