﻿using System;
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
        [MaxLength(100)]
        public string ContactPersonFirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string ContactPersonLastName { get; set; }

        [MaxLength(100)]
        public string CompanyTitle { get; set; }


        // Lazy loading 
        public virtual ICollection<CommunicationInfo> ?CommunicationInfos { get; set; }
    }
}
