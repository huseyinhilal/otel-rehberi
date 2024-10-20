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

       
        [HttpPost("bylocation")]
        public async Task<IActionResult> RequestReportByLocation([FromBody] string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                return BadRequest("Konum bilgisi zorunludur.");
            }

            var reportId = Guid.NewGuid();
            // Raporu hemen "Hazırlanıyor" durumuyla kaydediyoruz
            var report = new Report
            {
                Id=reportId,
                Location = location,
                Status = "Hazırlanıyor", // İlk durum "Hazırlanıyor"
                RequestedAt = DateTime.Now
            };

            _dbContext.Reports.Add(report);
            await _dbContext.SaveChangesAsync(); // Veritabanına kaydediyoruz


            // RabbitMQ kuyruğuna rapor isteğini gönder
            _rabbitMQProducer.SendReportToQueue(reportId, location);

            return Ok(new { Message = "Rapor oluşturma isteği kuyruğa gönderildi.", Location = location });
        }

        [HttpGet]
        public async Task<IActionResult> GetReports()
        {
            var reports = await _dbContext.Reports.ToListAsync();
            return Ok(reports);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportById(Guid id)
        {
            // Veritabanından ReportId'ye göre raporu bul
            var report = await _dbContext.Reports.FirstOrDefaultAsync(r => r.Id == id);

            // Eğer rapor bulunamazsa 404 döndür
            if (report == null)
            {
                return NotFound(new { Message = "Rapor bulunamadı." });
            }

            // Raporu başarıyla döndür
            return Ok(report);
        }
    }
}
