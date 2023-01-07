using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ParkPal.Common.Models.Database.Entities.Device;

namespace ParkPal.Common.Models.Database.Entities.Subscription
{
    [Table("Voucher", Schema = "Subscription")]
    public class Voucher
    {
        [Key]
        public int VoucherId { get; set; }
        
        [MaxLength(1000)]
        public string Code { get; set; }
        
        [Required]
        public bool Redeemed { get; set; }
    }
}