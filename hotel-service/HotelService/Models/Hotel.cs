using System;
using System.ComponentModel.DataAnnotations;

namespace HotelService.Models
{
    public class Hotel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Location { get; set; }

        [Required]
        [MaxLength(50)]
        public string ContactInfo { get; set; }
    }
}
