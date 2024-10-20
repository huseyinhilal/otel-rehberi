using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReportService.Data;
using ReportService.Models;

namespace ReportService.Services
{
    public class RabbitMQConsumerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConnectionFactory _factory;

        public RabbitMQConsumerService(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "report_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var report = JsonConvert.DeserializeObject<Report>(message);

                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ReportDbContext>();

                // Simüle olarak eklenen gecikme
                await Task.Delay(5000); // Raporun hazırlanma süresi

                // Raporun durumu güncelleniyor
                var reportInDb = await dbContext.Reports.FindAsync(report.Id);
                if (reportInDb != null)
                {
                    reportInDb.Status = "Tamamlandı";
                    await dbContext.SaveChangesAsync();
                }
            };

            channel.BasicConsume(queue: "report_queue", autoAck: true, consumer: consumer);

            await Task.CompletedTask;
        }
    }
}
