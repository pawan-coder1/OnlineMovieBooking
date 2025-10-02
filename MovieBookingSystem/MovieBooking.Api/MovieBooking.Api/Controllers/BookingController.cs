using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBookingSystem.Data;
using MovieBookingSystem.DTOs;
using MovieBookingSystem.Models;

namespace MovieBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Booking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings
                                 .Include(b => b.Showtime)
                                 .ThenInclude(s => s.Movie)
                                 .ToListAsync();
        }

        // GET: api/Booking/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetUserBookings(string userId)
        {
            var bookings = await _context.Bookings
                                         .Include(b => b.Showtime)
                                         .ThenInclude(s => s.Movie)
                                         .Where(b => b.UserId == userId)
                                         .ToListAsync();
            return bookings;
        }

        // POST: api/Booking
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var showtime = await _context.Showtimes.FindAsync(dto.ShowtimeId);
            if (showtime == null)
                return NotFound("Showtime not found");

            // Trim seats and validate
            var requestedSeats = dto.Seats.Select(s => s.Trim()).ToList();
            var bookedSeats = showtime.BookedSeats.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                  .Select(s => s.Trim())
                                                  .ToList();

            // Check for double booking
            if (requestedSeats.Any(s => bookedSeats.Contains(s)))
                return BadRequest("Some seats are already booked");

            // Transaction to prevent race conditions
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                bookedSeats.AddRange(requestedSeats);
                showtime.BookedSeats = string.Join(',', bookedSeats);

                var booking = new Booking
                {
                    ShowtimeId = dto.ShowtimeId,
                    UserId = dto.UserId,
                    Seats = string.Join(',', requestedSeats),
                    BookingTime = DateTime.Now,
                    PaymentStatus = "Pending"
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetBookings), new { id = booking.Id }, booking);
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Booking failed");
            }
        }

        // DELETE: api/Booking/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            var showtime = await _context.Showtimes.FindAsync(booking.ShowtimeId);
            if (showtime != null)
            {
                var bookedSeats = showtime.BookedSeats.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                      .Select(s => s.Trim())
                                                      .ToList();
                var seatsToRemove = booking.Seats.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                .Select(s => s.Trim());
                bookedSeats = bookedSeats.Except(seatsToRemove).ToList();
                showtime.BookedSeats = string.Join(',', bookedSeats);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return NoContent();
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Failed to cancel booking");
            }
        }
    }
}
