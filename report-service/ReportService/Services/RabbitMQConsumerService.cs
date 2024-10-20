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
                // ReportId kontrolü ve parse işlemi
                if (reportRequest?.ReportId == null || !Guid.TryParse((string)reportRequest.ReportId.ToString(), out reportId))
                {
                    Console.WriteLine("ReportId geçersiz veya null: " + reportRequest?.ReportId);
                    return; // ReportId null veya geçersizse işlemi durdur.
                }

                var location = (string)reportRequest.Location;

                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ReportDbContext>();

                // Mevcut raporu veritabanında bul (yeni rapor oluşturmak yerine)
                var reportInDb = await dbContext.Reports.FindAsync(reportId);
                if (reportInDb == null)
                {
                    Console.WriteLine($"Rapor veritabanında bulunamadı: {reportId}");
                    return;
                }

                // HotelService'ten otel bilgilerini al
                var hotels = await _hotelServiceClient.GetHotelsByLocation(location);

                if (hotels == null || !hotels.Any())
                {
                    Console.WriteLine($"'{location}' konumunda otel bulunamadı.");
                    return;
                }

                // Raporu güncelle
                reportInDb.HotelCount = hotels.Count;
                reportInDb.ContactInfoCount = hotels.Sum(h => h.ContactInfo.Length); // İletişim bilgileri sayısı
                reportInDb.Status = "Tamamlandı"; // Raporu tamamlanmış olarak işaretle

                // Simüle edilmiş kısa bekleme
                await Task.Delay(5000, stoppingToken);

                await dbContext.SaveChangesAsync();
                Console.WriteLine($"Rapor güncellendi ve tamamlandı: {reportInDb.Id}");

                // Mesajı onayla
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                channel.BasicConsume(queue: "report_queue", autoAck: false, consumer: consumer);

                // Kısa bekleme
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
