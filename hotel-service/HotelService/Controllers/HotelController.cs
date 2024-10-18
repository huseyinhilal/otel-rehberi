using HotelService.Models;
using HotelService.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotelService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly HotelRepository _repository;

        public HotelController(HotelRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHotels()
        {
            var hotels = await _repository.GetAllHotelsAsync();
            return Ok(hotels);
        }

        [HttpPost]
        public async Task<IActionResult> AddHotel([FromBody] Hotel hotel)
        {
            await _repository.AddHotelAsync(hotel);
            return CreatedAtAction(nameof(GetAllHotels), new { id = hotel.Id }, hotel);
        }
    }
}
