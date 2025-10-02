namespace MovieBookingSystem.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public DateTime ReleaseDate { get; set; }

        // New properties
        public string Description { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
    }
}
