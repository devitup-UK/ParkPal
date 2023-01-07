using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ParkPal.Common.Models.Database.Entities.Device;

namespace ParkPal.Common.Models.Database.Entities.Notification
{
    [Table("Item", Schema = "Notification")]
    public class Item
    {
        [Key]
        public int ItemId { get; set; }
        
        public int MinuteInterval { get; set; }
        
        public int TypeId { get; set; }

        [MaxLength(1000)]
        public string? AttractionId { get; set; }
        
        [MaxLength(1000)]
        public string ParkId { get; set; }
        
        public int CriteriaType { get; set; }
        
        public int WaitTime { get; set; }
        
        public bool Enabled { get; set; }
        
        [ForeignKey("Subscription")]
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
    }
}