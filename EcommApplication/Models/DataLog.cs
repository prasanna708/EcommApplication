using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommApplication.Models
{
    public class DataLog
    {
        public int? Id { get; set; }
        public string TableEffected { get; set; }
        public int? PropertyId { get; set; }
        public string PropertyEffected { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string UserId { get; set; }
        public DateTime ActivityDateAndTime { get; set; }
    }
}