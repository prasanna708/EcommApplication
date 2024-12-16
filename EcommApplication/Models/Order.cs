using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace EcommApplication.Models
{
    public class Order
    {
        [Key]
        public int? OrderId { get; set; }

        [ForeignKey("Product")]
        public int? ProductId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public string Name { get; set; }
        public int? Quantity { get; set; }

        [DisplayName("Order Date And Time")]
        public DateTime OrderDateAndTime { get; set; }
        public double? TotalPrice { get; set; }
        public string Photo { get; set; }
        public bool IsActive { get; set; }
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
        
    }
}