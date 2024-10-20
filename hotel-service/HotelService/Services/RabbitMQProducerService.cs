﻿using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace HotelService.Services
{
    public class RabbitMQProducerService
    {
        private readonly ConnectionFactory _factory;

        public RabbitMQProducerService(IConfiguration configuration)
        {
            _factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };
        }

        public void SendReportRequestToQueue(string location)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "report_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var message = JsonConvert.SerializeObject(new { Location = location });
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "report_queue", basicProperties: null, body: body);
        }
    }
}