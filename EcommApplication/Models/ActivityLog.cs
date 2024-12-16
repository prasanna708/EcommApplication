using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommApplication.Models
{
    public class ActivityLog
    {
        public int? Id { get; set; }
        public string UserId { get; set; }
        public DateTime ActivityDateAndTime { get; set; }
        public string Action { get; set; }
        public string TableEffected { get; set; }
        public int? TableId { get; set; }
        public string Details { get; set; }
    }
}