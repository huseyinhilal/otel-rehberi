using HotelService.Data;
using HotelService.Models;
using HotelService.Services;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateHotel([FromBody] Hotel hotel)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(); // begin Transaction
            try
            {
                Log.Information("Attempt to create a new hotel: {@Hotel}", hotel);// Logging
                _context.Hotels.Add(hotel);
                await _context.SaveChangesAsync(); // Save Hotel

                // Transaction commit (Success case)
                await transaction.CommitAsync();
                return CreatedAtAction(nameof(GetHotels), new { id = hotel.Id }, hotel);
            }
            catch (Exception ex)
            {
                // Rollback on Error
                await transaction.RollbackAsync();
                Log.Error("ERROR:CreateHotel An Error occured during Hotel Creation: {ErrorMessage}", ex.Message);// Logging
                return StatusCode(500, "ERROR:CreateHotel .");
            }
        }

        // Update hotel 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] Hotel updatedHotel)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(); // begin Transaction
            try
            {
                Log.Information("Attempt to Updating the Hotel : {@id}", id); // Logging
                var hotel = await _context.Hotels.FindAsync(id);
                if (hotel == null) return NotFound();

                hotel.Name = updatedHotel.Name;
                hotel.Location = updatedHotel.Location;
                hotel.ContactPersonFirstName = updatedHotel.ContactPersonFirstName;
                hotel.ContactPersonLastName = updatedHotel.ContactPersonLastName;
                hotel.ContactInfo = updatedHotel.ContactInfo;

                await _context.SaveChangesAsync();
                // Transaction commit (Success case)
                await transaction.CommitAsync();
                return NoContent();
            }
            catch(Exception ex)
            {

                // Rollback on Error
                await transaction.RollbackAsync();
                Log.Error("ERROR:UpdateHotel An Error occured during Hotel Update: {ErrorMessage}", ex.Message);// Logging
                return StatusCode(500, "ERROR:UpdateHotel .");
            }
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

        // Get hotel by location
        [HttpGet("bylocation")]
        public async Task<IActionResult> GetHotelsByLocation([FromQuery] string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                return BadRequest("Location parametresi zorunludur.");
            }

            // Veritabanından ilgili konuma ait otelleri getirir
            var hotels = await _context.Hotels
                                       .Where(h => h.Location == location)
                                       .ToListAsync();

            if (hotels == null || !hotels.Any())
            {
                return NotFound($"'{location}' konumunda otel bulunamadı.");
            }

            return Ok(hotels); // İlgili otelleri döner
        }
    }

}
