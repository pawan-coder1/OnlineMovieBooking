namespace MovieBookingSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;  // FK to IdentityUser
        public int ShowtimeId { get; set; }                  // FK to Showtime
        public Showtime Showtime { get; set; } = null!;

        // Comma-separated seat numbers
        public string Seats { get; set; } = string.Empty;

        public DateTime BookingTime { get; set; } = DateTime.Now;

        // Payment status stub
        public string PaymentStatus { get; set; } = "Pending";
    }
}
