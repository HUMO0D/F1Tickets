namespace F1Tickets.Models
{
    public class RaceResultsViewModel
    {
        public string RaceName { get; set; }
        public string CircuitName { get; set; }
        public string RaceDate { get; set; }
        public List<RaceResultViewModel> Results { get; set; }
    }
}
