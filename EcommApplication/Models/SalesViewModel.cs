using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommApplication.Models
{
    public class SalesViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<PurchaseAuditLogs> FilteredSales { get; set; }

    }
}