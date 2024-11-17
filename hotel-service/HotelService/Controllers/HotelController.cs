using HotelService.Interfaces;
using HotelService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HotelService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelController(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        // List all hotels
        [HttpGet]
        public async Task<IActionResult> GetHotels(int page = 1, int pageSize = 10)
        {
            var response = await _hotelRepository.GetAllHotelsAsync(page, pageSize);
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

        // Update hotel
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] Hotel updatedHotel)
        {
            await _hotelRepository.UpdateHotelAsync(id, updatedHotel);
            return NoContent();
        }

        // Delete hotel
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
            var hotel = await _hotelRepository.GetHotelByIdAsync(id);
            if (hotel == null) return NotFound();
            return Ok(hotel);
        }

        // Get hotel by location
        [HttpGet("bylocation")]
        public async Task<IActionResult> GetHotelsByLocation([FromQuery] string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                return BadRequest("Location parameter is required.");
            }

            var hotels = await _hotelRepository.GetHotelsByLocationAsync(location);
            if (hotels == null || !hotels.Any())
            {
                return NotFound($"No hotels found at '{location}' location.");
            }

            return Ok(hotels);
        }
    }
}
