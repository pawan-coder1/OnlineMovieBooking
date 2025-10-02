using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MovieBookingSystem.DTOs
{
    public class CreateBookingDto
    {
        [Required]
        public int ShowtimeId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one seat must be selected.")]
        [SeatFormatValidation(ErrorMessage = "Seats must be in format: Letter followed by number (e.g., A1, B12).")]
        public List<string> Seats { get; set; } = new();

        [Required]
        public string UserId { get; set; } = string.Empty;
    }

    // Custom validation attribute for seat format
    public class SeatFormatValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is List<string> seats)
            {
                var regex = new Regex(@"^[A-Z]\d{1,2}$"); // e.g., A1, B12
                foreach (var seat in seats)
                {
                    if (!regex.IsMatch(seat.Trim()))
                        return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
