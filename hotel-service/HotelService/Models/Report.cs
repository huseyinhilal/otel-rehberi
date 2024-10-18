using System;

namespace HotelService.Models
{
    public class Report
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Location { get; set; }
        public int HotelCount { get; set; }
        public int ContactCount { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Hazırlanıyor"; // İlk başta "Hazırlanıyor" olacak
    }
}
