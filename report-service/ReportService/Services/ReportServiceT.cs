using Microsoft.EntityFrameworkCore;
using ReportService.Data;
using ReportService.Interfaces;
using ReportService.Models;

namespace ReportService.Services
{
    // ReportServiceT.cs
    public class ReportServiceT : IReportServiceT
    {
        private readonly ReportDbContext _dbContext;
        private readonly RabbitMQProducerService _rabbitMQProducer;

        public ReportServiceT(ReportDbContext dbContext, RabbitMQProducerService rabbitMQProducer)
        {
            _dbContext = dbContext;
            _rabbitMQProducer = rabbitMQProducer;
        }

        public async Task<Guid> CreateReportByLocationAsync(string location)
        {
            if (string.IsNullOrEmpty(location))
                throw new ArgumentException("Konum bilgisi zorunludur.");

            var reportId = Guid.NewGuid();
            var report = new Report
            {
                Id = reportId,
                Location = location,
                Status = "Hazırlanıyor",
                RequestedAt = DateTime.Now
            };

            _dbContext.Reports.Add(report);
            await _dbContext.SaveChangesAsync();

            _rabbitMQProducer.SendReportToQueue(reportId, location);

            return reportId;
        }

        public async Task<List<Report>> GetReportsAsync()
        {
            return await _dbContext.Reports.ToListAsync();
        }

        public async Task<Report?> GetReportByIdAsync(Guid id)
        {
            return await _dbContext.Reports.FirstOrDefaultAsync(r => r.Id == id);
        }
    }


}
