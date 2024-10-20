using Microsoft.AspNetCore.Mvc;
using HotelService.Data;
using HotelService.Models;

namespace HotelService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunicationInfoController : ControllerBase
    {
        private readonly HotelDbContext _dbContext;

        public CommunicationInfoController(HotelDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // CommunicationInfo ekleme
        [HttpPost("{hotelId}")]
        public async Task<IActionResult> AddCommunicationInfo(Guid hotelId, [FromBody] CommunicationInfoDto communicationInfoDto)
        {
            // Otel var mı kontrol et
            var hotel = await _dbContext.Hotels.FindAsync(hotelId);
            if (hotel == null)
            {
                return NotFound(new { Message = "Otel bulunamadı." });
            }

            var communicationInfo = new CommunicationInfo
            {
                HotelId = hotelId, // Otel ile ilişkilendir
                InfoType = communicationInfoDto.InfoType,
                InfoDetails = communicationInfoDto.InfoDetails
            };

            // Veritabanına kaydet
            _dbContext.CommunicationInfo.Add(communicationInfo);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "İletişim bilgisi başarıyla eklendi.", CommunicationInfoId = communicationInfo.Id });
        }
    }

    public class CommunicationInfoDto
    {
        public string InfoType { get; set; } // İletişim türü (Telefon, Email, vs.)
        public string InfoDetails { get; set; } // İletişim bilgisi (Telefon numarası, Email, vs.)
    }
}
