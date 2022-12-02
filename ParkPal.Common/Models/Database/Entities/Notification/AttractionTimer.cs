using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ParkPal.Common.Models.Database.Entities.Device;

namespace ParkPal.Common.Models.Database.Entities.Notification
{
    [Obsolete("AttractionTimer is due to be removed and is therefore obsolete, please use Notification.Item going forwards.")]
    [Table("AttractionTimer", Schema = "Notification")]
    public class AttractionTimer
    {
        [Key]
        public int AttractionTimerId { get; set; }
        
        public int MinuteInterval { get; set; }
        
        [MaxLength(1000)]
        public string ParkId { get; set; }
        
        [MaxLength(1000)]
        public string AttractionId { get; set; }
        
        public int CriteriaType { get; set; }
        
        public int WaitTime { get; set; }
        
        public bool Enabled { get; set; }
        
        [ForeignKey("Subscription")]
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
    }
}