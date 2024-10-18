using HotelService.Models;
using HotelService.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HotelService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ReportRepository _repository;

        public ReportController(ReportRepository repository)
        {
            _repository = repository;
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportById(Guid id)
        {
            var report = await _repository.GetReportByIdAsync(id);
            if (report == null)
            {
                return NotFound(); // Rapor bulunamazsa 404 
            }
            return Ok(report); 
        }

        // Tüm raporları listele
        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            var reports = await _repository.GetAllReportsAsync();
            return Ok(reports);
        }

        // Yeni rapor oluştur
        [HttpPost]
        public async Task<IActionResult> CreateReport([FromQuery] string location)
        {
            var report = await _repository.GenerateReportAsync(location);
            return Ok(report);
        }
    }


}
