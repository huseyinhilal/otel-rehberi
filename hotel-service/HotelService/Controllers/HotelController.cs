using HotelService.Data;
using HotelService.Models;
using HotelService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Threading.Tasks;

namespace HotelService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public HotelController(HotelDbContext context)
        {
            _context = context;
        }

        // List all hotels
        [HttpGet]
        public async Task<IActionResult> GetHotels()
        {
            var hotels = await _context.Hotels.ToListAsync();
            return Ok(hotels);
        }

        // Create new hotel
        [HttpPost]
        public async Task<IActionResult> CreateHotel([FromBody] Hotel hotel)
        {
            Log.Information("A new hotel created: {@Hotel}", hotel); // Logging
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetHotels), new { id = hotel.Id }, hotel);
        }

        // Update hotel 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] Hotel updatedHotel)
        {
            Log.Information("Updating the Hotel : {@id}", id); // Logging
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return NotFound();

            hotel.Name = updatedHotel.Name;
            hotel.Location = updatedHotel.Location;
            hotel.ContactPersonFirstName = updatedHotel.ContactPersonFirstName;
            hotel.ContactPersonLastName = updatedHotel.ContactPersonLastName;
            hotel.ContactInfo = updatedHotel.ContactInfo;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Delete hotel
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(Guid id)
        {
            Log.Information("Deleting the Hotel : {@id}", id); // Logging
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return NotFound();

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Get hotel by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotelById(Guid id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return NotFound();
            return Ok(hotel);
        }
    }

}
