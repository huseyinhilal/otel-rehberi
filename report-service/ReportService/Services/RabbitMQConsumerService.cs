using System.Text;
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
        private readonly HotelServiceClient _hotelServiceClient;

        public RabbitMQConsumerService(IConfiguration configuration, IServiceScopeFactory scopeFactory, HotelServiceClient hotelServiceClient)
        {
            _factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };
            _scopeFactory = scopeFactory;
            _hotelServiceClient = hotelServiceClient;
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

                dynamic reportRequest = null;
                try
                {
                    reportRequest = JsonConvert.DeserializeObject<dynamic>(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Deserialization hatası: {ex.Message}");
                    return;
                }

                Guid reportId = Guid.Empty;
                // ReportId validation and parsing
                if (reportRequest?.ReportId == null || !Guid.TryParse((string)reportRequest.ReportId.ToString(), out reportId))
                {
                    Console.WriteLine("ReportId is null or invalid: " + reportRequest?.ReportId);
                    return; // Stop the process if ReportId is null or invalid.
                }

                var location = (string)reportRequest.Location;

                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ReportDbContext>();

                // Find the existing report in the database (instead of creating a new one)
                var reportInDb = await dbContext.Reports.FindAsync(reportId);
                if (reportInDb == null)
                {
                    Console.WriteLine($"Report not found in the database: {reportId}");
                    return;
                }

                // Retrieve hotel information from HotelService
                var hotels = await _hotelServiceClient.GetHotelsByLocation(location);

                if (hotels == null || !hotels.Any())
                {
                    Console.WriteLine($"'{location}' konumunda otel bulunamadı.");
                    return;
                }

                // Update the report
                reportInDb.HotelCount = hotels.Count;
                //reportInDb.ContactInfoCount = hotels.Sum(h => h.ContactInfo.Length); 
                reportInDb.Status = "Completed"; // Mark the report as Completed

                // Simulated short delay
                await Task.Delay(5000, stoppingToken);

                await dbContext.SaveChangesAsync();
                Console.WriteLine($"The report has been updated and completed: {reportInDb.Id}");

                // Acknowledge the message
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                channel.BasicConsume(queue: "report_queue", autoAck: false, consumer: consumer);

                // Short delay
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
