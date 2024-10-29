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
        private readonly HotelRepository _hotelRepository;
        private readonly HotelDbContext _context;

        public HotelController(HotelDbContext context, HotelRepository hotelRepository)
        {
            _context = context;
            _hotelRepository = hotelRepository;
        }


        // List all hotels
        [HttpGet]
        public async Task<IActionResult> GetHotels(int page = 1, int pageSize = 10)
        {
            var totalRecords = await _context.Hotels.CountAsync();
            var hotels = await _context.Hotels
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new
            {
                TotalRecords = totalRecords,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                Hotels = hotels
            };

            return Ok(response);
        }


        // Create new hotel
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateHotel([FromBody] Hotel hotel)
        {
            await _hotelRepository.AddHotelAsync(hotel);
            return CreatedAtAction(nameof(GetHotels), new { id = hotel.Id }, hotel);
        }

        //update hotel
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] Hotel updatedHotel)
        {
            await _hotelRepository.UpdateHotelAsync(id, updatedHotel);
            return NoContent();
        }

        //delete hotel
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(Guid id)
        {
            await _hotelRepository.DeleteHotelAsync(id);
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

            return Ok(hotels);
        }
    }

}
