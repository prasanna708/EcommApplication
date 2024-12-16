using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EcommApplication.Models
{
    public class Product
    {
        [Key]
        public int? ProductId { get; set; }

        [DisplayName("Product Name")]
        public string Name { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public string Photo { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}