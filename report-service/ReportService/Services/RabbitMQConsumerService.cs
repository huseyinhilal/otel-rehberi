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
            try
            {
                using var connection = _factory.CreateConnection();
                using var channel = connection.CreateModel();
                Console.WriteLine("RabbitMQ bağlantısı başarılı, kuyruk dinleniyor...");

                channel.QueueDeclare(queue: "report_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                Console.WriteLine("Kuyruk başarıyla tanımlandı: report_queue");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    Console.WriteLine("Mesaj alınıyor...");
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Mesaj içeriği: " + message);

                    var report = JsonConvert.DeserializeObject<Report>(message);
                    Console.WriteLine($"Deserializasyon başarılı, rapor id: {report.Id}");

                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<ReportDbContext>();

                        // Simüle olarak eklenen gecikme
                        await Task.Delay(5000); // Raporun hazırlanma süresi
                        Console.WriteLine("Rapor hazırlama süresi tamamlandı.");

                        // Raporun durumu güncelleniyor
                        var reportInDb = await dbContext.Reports.FindAsync(report.Id);
                        if (reportInDb != null)
                        {
                            reportInDb.Status = "Tamamlandı";
                            await dbContext.SaveChangesAsync();
                            Console.WriteLine($"Rapor başarıyla güncellendi: {reportInDb.Id}");
                        }
                        else
                        {
                            Console.WriteLine($"Rapor veritabanında bulunamadı: {report.Id}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Veritabanı işlemi sırasında hata oluştu: {ex.Message}");
                    }

                    // Mesaj işlendikten sonra onayla
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    Console.WriteLine("Mesaj başarıyla işlenip onaylandı.");
                };

                while (!stoppingToken.IsCancellationRequested)
                {
                    channel.BasicConsume(queue: "report_queue", autoAck: false, consumer: consumer);
                    Console.WriteLine("Kuyruk dinleniyor...");

                    // Arka planda çalışmaya devam etmek için kısa bir bekleme
                    await Task.Delay(1000, stoppingToken);
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RabbitMQ bağlantısı sırasında hata oluştu: {ex.Message}");
            }
        }
    }
}
