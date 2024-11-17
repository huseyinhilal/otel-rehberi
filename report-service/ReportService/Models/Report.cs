using System.ComponentModel.DataAnnotations;

namespace ReportService.Models
{
    public class Report
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Location { get; set; }

        [Required]
        public int HotelCount { get; set; }

        [Required]
        public int ContactInfoCount { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Preparing"; 
    }

}
