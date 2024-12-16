using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EcommApplication.Models
{
    public class PurchaseAuditLogs
    {
        [Key]
        public int? OrderNumber { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public string UserId { get; set; }
        public string ProductName { get; set; }
        public int? Quantity { get; set; }
        public double? TotalPrice { get; set; }
        public DateTime OrderDateAndTime { get; set; }
        public string Photo { get; set; } 
    }
}