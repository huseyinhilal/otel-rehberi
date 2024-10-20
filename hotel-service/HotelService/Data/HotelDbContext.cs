using HotelService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelService.Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) { }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<CommunicationInfo> CommunicationInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CommunicationInfo>()
                .HasOne(ci => ci.Hotel)
                .WithMany(h => h.CommunicationInfos)
                .HasForeignKey(ci => ci.HotelId);
        }
    }
}
