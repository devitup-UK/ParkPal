using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ParkPal.Common.Models.Database.Entities.Device;

namespace ParkPal.Common.Models.Database.Entities.Notification
{
    [Table("Subscription", Schema = "Notification")]
    public class Subscription
    {
        [Key]
        public int SubscriptionId { get; set; }
        
        [ForeignKey("Token")]
        public int TokenId { get; set; }
        public Token? Token { get; set; }
        
        [MaxLength(1000)]
        public string PlayerId { get; set; }
        
        public virtual IEnumerable<AttractionTimer> AttractionTimers { get; set; }
    }
}