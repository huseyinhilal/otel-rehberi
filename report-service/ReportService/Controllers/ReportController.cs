using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReportService.Data;
using ReportService.Models;
using ReportService.Services;

namespace ReportService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ReportDbContext _dbContext;
        private readonly RabbitMQProducerService _rabbitMQProducer;

        public ReportController(ReportDbContext dbContext, RabbitMQProducerService rabbitMQProducer)
        {
            _dbContext = dbContext;
            _rabbitMQProducer = rabbitMQProducer;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] Report report)
        {
            _dbContext.Reports.Add(report);
            await _dbContext.SaveChangesAsync();

            // Raporu RabbitMQ kuyruğuna gönder
            _rabbitMQProducer.SendReportToQueue(report);

            return Ok(report);
        }

        [HttpGet]
        public async Task<IActionResult> GetReports()
        {
            var reports = await _dbContext.Reports.ToListAsync();
            return Ok(reports);
        }
    }
}
