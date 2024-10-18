using HotelService.Data;
using HotelService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HotelService.Services
{
    public class ReportRepository
    {
        private readonly HotelDbContext _context;

        public ReportRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task AddReportAsync(Report report)
        {
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Report>> GetAllReportsAsync()
        {
            return await _context.Reports.ToListAsync();
        }

        public async Task<Report> GetReportByIdAsync(Guid id)
        {
            return await _context.Reports.FindAsync(id);
        }
    }
}
