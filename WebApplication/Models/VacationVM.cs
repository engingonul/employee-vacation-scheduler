using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class VacationVM
    {
        public int id { get; set; }
        public int emp_id { get; set; }
        public string type { get; set; }
        public System.DateTime start_date { get; set; }
        public System.DateTime end_date { get; set; }
        public string reason { get; set; }
        public string status { get; set; }
        public string approved_by { get; set; }
        public string accessible_num { get; set; }
        public Nullable<double> days { get; set; }
    }
}