using System;
using System.ComponentModel.DataAnnotations;

namespace HotelService.Models
{
    public class Report
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();  

        [Required]  
        [MaxLength(100)]  
        public string Location { get; set; } 

        public int HotelCount { get; set; }  
        public int PhoneNumberCount { get; set; }  

        [Required] 
        public DateTime RequestedAt { get; set; } = DateTime.Now;  

        [Required]  
        [MaxLength(20)]  
        public string Status { get; set; } = "Hazırlanıyor";  
    }
}
