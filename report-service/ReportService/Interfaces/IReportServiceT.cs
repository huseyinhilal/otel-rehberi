using ReportService.Models;

namespace ReportService.Interfaces
{
    public interface IReportServiceT
    {
        Task<Guid> CreateReportByLocationAsync(string location);
        Task<List<Report>> GetReportsAsync();
        Task<Report?> GetReportByIdAsync(Guid id);
    }

}
