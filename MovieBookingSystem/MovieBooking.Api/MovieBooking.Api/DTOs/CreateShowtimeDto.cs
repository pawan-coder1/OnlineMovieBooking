namespace MovieBookingSystem.DTOs
{
    public class CreateShowtimeDto
    {
        public int MovieId { get; set; }
        public string Theater { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public int TotalSeats { get; set; }
    }
}
