﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReportService.Data;
using ReportService.Interfaces;
using ReportService.Models;
using ReportService.Services;
using Serilog;

namespace ReportService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ReportDbContext _dbContext;
        private readonly RabbitMQProducerService _rabbitMQProducer;
        private readonly IReportProcessingService _reportService;

        public ReportController(ReportDbContext dbContext, RabbitMQProducerService rabbitMQProducer, IReportProcessingService reportService)
        {
            _dbContext = dbContext;
            _rabbitMQProducer = rabbitMQProducer;
            _reportService = reportService;
        }


        [HttpPost("bylocation")]
        public async Task<IActionResult> RequestReportByLocation([FromBody] string location)
        {
            var reportId = await _reportService.CreateReportByLocationAsync(location);
            return Ok(new { Message = "Report creation request sent to queue.", Location = location, ReportId = reportId });
        }

        [HttpGet]
        public async Task<IActionResult> GetReports()
        {
            var reports = await _reportService.GetReportsAsync();
            return Ok(reports);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportById(Guid id)
        {
            var report = await _reportService.GetReportByIdAsync(id);
            if (report == null)
            {
                return NotFound(new { Message = "Report not found" });
            }
            return Ok(report);
        }
    }
}
