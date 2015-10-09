using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class EmployeeVM
    {
        public int id { get; set; }

        [Display(Name = "Name")]
        public string name { get; set; }

        [Display(Name = "Surame")]
        public string surname { get; set; }

        [Display(Name = "Phone Number")]
        [RegularExpression(@"^(0(\d{3})(\d{3})(\d{2})(\d{2}))$")]
        public string phoneNum { get; set; }

        [Display(Name = "Username")]
        public string username { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime startDate { get; set; }

        [Display(Name = "Human Resources")]
        public bool role { get; set; }

        [Display(Name = "Total Eligible Days (Vacation)")]
        public Nullable<double> eligibleDays { get; set; }

        [Display(Name = "Days Left (Vacation)")]
        public Nullable<double> daysLeft { get; set; }

        [DefaultValue(true)]
        public bool isActive { get; set; }
    }
}