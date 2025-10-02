using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBookingSystem.Data;
using MovieBookingSystem.DTOs;
using MovieBookingSystem.Models;

namespace MovieBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowtimeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShowtimeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Showtime
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Showtime>>> GetShowtimes()
        {
            return await _context.Showtimes.Include(s => s.Movie).ToListAsync();
        }

        // GET: api/Showtime/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Showtime>> GetShowtime(int id)
        {
            var showtime = await _context.Showtimes.Include(s => s.Movie)
                                                   .FirstOrDefaultAsync(s => s.Id == id);
            if (showtime == null)
                return NotFound();

            return showtime;
        }

        // POST: api/Showtime
        [HttpPost]
        public async Task<ActionResult<Showtime>> CreateShowtime(CreateShowtimeDto dto)
        {
            var movie = await _context.Movies.FindAsync(dto.MovieId);
            if (movie == null)
                return BadRequest("Movie not found");

            var showtime = new Showtime
            {
                MovieId = dto.MovieId,
                Theater = dto.Theater,
                StartTime = dto.StartTime,
                TotalSeats = dto.TotalSeats
            };

            _context.Showtimes.Add(showtime);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShowtime), new { id = showtime.Id }, showtime);
        }

        // PUT: api/Showtime/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShowtime(int id, CreateShowtimeDto dto)
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null)
                return NotFound();

            showtime.MovieId = dto.MovieId;
            showtime.Theater = dto.Theater;
            showtime.StartTime = dto.StartTime;
            showtime.TotalSeats = dto.TotalSeats;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Showtime/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShowtime(int id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null)
                return NotFound();

            _context.Showtimes.Remove(showtime);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
