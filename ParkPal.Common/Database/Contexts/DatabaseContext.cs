using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using ParkPal.Common.Models.Configuration;
using ParkPal.Common.Models.Database.Entities;
using ParkPal.Common.Models.Database.Entities.Device;
using ParkPal.Common.Models.Database.Entities.Log;
using ParkPal.Common.Models.Database.Entities.Notification;

namespace ParkPal.Common.Database.Contexts
{
    public class DatabaseContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        // Database Tables
        // Device Schema
        public DbSet<Token> Tokens { get; set; }

        // Notification Schema
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<AttractionTimer> AttractionTimers { get; set; }
        
        // Log Schema
        public DbSet<Item>? LogItems { get; set; }

        public DatabaseContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Settings.SQLConnectionString);
        }
    }
}