using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ParkPal.Common.Models.Database.Entities.Log
{
    [Table("Item", Schema = "Log")]
    public class Item
    {
        [Key]
        public int ItemId { get; set; }
        public int ThreadId { get; set; }
        public string? LogLevel { get; set; }
        public int? EventId { get; set; }
        public string? EventName { get; set; }
        public string? Message { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? ExceptionStackTrace { get; set; }
        public string? ExceptionSource { get; set; }
        public string? HostName { get; set; }
        public string? ApplicationName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}