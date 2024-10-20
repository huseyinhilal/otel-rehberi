using Microsoft.AspNetCore.Mvc;
using HotelService.Services;

namespace HotelService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly RabbitMQProducerService _rabbitMQProducerService;

        public ReportController(RabbitMQProducerService rabbitMQProducerService)
        {
            _rabbitMQProducerService = rabbitMQProducerService;
        }

        [HttpPost]
        public IActionResult RequestReport([FromBody] string location)
        {
            _rabbitMQProducerService.SendReportRequestToQueue(location);
            return Ok(new { Message = "Rapor talebi gönderildi", Location = location });
        }
    }
}
