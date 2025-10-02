using System.ComponentModel.DataAnnotations.Schema;

namespace MovieBookingSystem.Models
{
    public class Showtime
    {
        public int Id { get; set; }

        // FK to Movie
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        // Theater or Screen name
        public string Theater { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        // Total seats in this show
        public int TotalSeats { get; set; }

        // Seats booked (comma-separated seat numbers for simplicity)
        public string BookedSeats { get; set; } = string.Empty;
    }
}
