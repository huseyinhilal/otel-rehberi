﻿using HotelService.Data;
using HotelService.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HotelService.Services
{
    public class ReportRepository
    {
        private readonly HotelDbContext _context;
        private readonly IServiceScopeFactory _scopeFactory;

        public ReportRepository(HotelDbContext context, IServiceScopeFactory scopeFactory)
        {
            _context = context;
            _scopeFactory = scopeFactory; // Scoped context üretmek için kullanacağız
        }

        public async Task<Report> GenerateReportAsync(string location)
        {
            // İlk olarak raporu "Hazırlanıyor" durumu ile oluşturuyoruz
            var report = new Report
            {
                Location = location,
                HotelCount = _context.Hotels.Count(h => h.Location == location),
                PhoneNumberCount = _context.Hotels.Count(h => h.Location == location), // Basit örnek
                Status = "Hazırlanıyor"
            };

            _context.Add(report);
            await _context.SaveChangesAsync();

            // Raporu arka planda tamamlamak için bir görev oluşturuyoruz
            Task.Run(async () =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();

                    await Task.Delay(5000); // Simülasyon: 5 saniye bekliyoruz
                    report.Status = "Tamamlandı";
                    scopedContext.Reports.Update(report);
                    await scopedContext.SaveChangesAsync();
                }
            });

            return report;
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
