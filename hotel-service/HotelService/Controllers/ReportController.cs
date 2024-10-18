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

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] string location)
        {
            var report = new Report
            {
                Location = location
            };
            await _repository.AddReportAsync(report);
            return Ok(report);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            var reports = await _repository.GetAllReportsAsync();
            return Ok(reports);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportById(Guid id)
        {
            var report = await _repository.GetReportByIdAsync(id);
            if (report == null)
                return NotFound();

            return Ok(report);
        }
    }
}
