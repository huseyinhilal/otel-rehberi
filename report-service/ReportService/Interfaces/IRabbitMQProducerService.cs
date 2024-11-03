public interface IRabbitMQProducerService
{
    void SendReportToQueue(Guid reportId, string location);
}