using Microsoft.AspNetCore.Mvc;
using HotelService.Data;
using HotelService.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

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

        // Add CommunicationInfo
        [HttpPost("{hotelId}")]
        public async Task<IActionResult> AddCommunicationInfo(Guid hotelId, [FromBody] CommunicationInfoDto communicationInfoDto)
        {
            // Check if the hotel exists
            var hotel = await _dbContext.Hotels.FindAsync(hotelId);
            if (hotel == null)
            {
                return NotFound(new { Message = "Hotel not found." });
            }

            var communicationInfo = new CommunicationInfo
            {
                HotelId = hotelId, // Associate with the hotel
                InfoType = communicationInfoDto.InfoType,
                InfoDetails = communicationInfoDto.InfoDetails
            };

            // Save to the database
            _dbContext.CommunicationInfo.Add(communicationInfo);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Communication info added successfully.", CommunicationInfoId = communicationInfo.Id });
        }

        // Delete CommunicationInfo
        [HttpDelete("{communicationInfoId}")]
        public async Task<IActionResult> DeleteCommunicationInfo(Guid communicationInfoId)
        {
            try
            {
                // Find the communication info
                var communicationInfo = await _dbContext.CommunicationInfo.FindAsync(communicationInfoId);
                if (communicationInfo == null)
                {
                    return NotFound($"Communication info with ID {communicationInfoId} not found.");
                }

                // Delete the communication info from the database
                _dbContext.CommunicationInfo.Remove(communicationInfo);
                await _dbContext.SaveChangesAsync();

                return Ok($"Communication info with ID {communicationInfoId} deleted successfully.");
            }
            catch (Exception ex)
            {
                // Log and return the error
                Log.Error("ERROR:DeleteCommunicationInfo - An error occurred while deleting communication info: {ErrorMessage}", ex.Message);
                return StatusCode(500, "ERROR:DeleteCommunicationInfo.");
            }
        }
    }

    public class CommunicationInfoDto
    {
        public string InfoType { get; set; } // Communication type (Phone, Email, etc.)
        public string InfoDetails { get; set; } // Communication details (Phone number, Email, etc.)
    }
}
