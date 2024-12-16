using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EcommApplication.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("EcommerceDb")
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PurchaseAuditLogs> PurchaseAuditLogs { get; set; }
        public DbSet<ErrorLogs> ErrorLogs { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<DataLog> DataLogs { get; set; }
    }
}