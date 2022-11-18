using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ParkPal.Common.Models.Database.Entities.Device
{
    [Table("Token", Schema = "Device")]
    public class Token
    {
        [Key]
        public int TokenId { get; set; }
        
        [MaxLength(1000)]
        public string? Value { get; set; }
    }
}