using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class EmployeeVM
    {
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string phoneNum { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public System.DateTime startDate { get; set; }
        public bool role { get; set; }
        public Nullable<double> eligibleDays { get; set; }
        public Nullable<double> daysLeft { get; set; }
    }
}