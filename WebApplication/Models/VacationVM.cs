﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class VacationVM
    {
        public int id { get; set; }
        public int emp_id { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string type { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime start_date { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime end_date { get; set; }

        [Required]
        [Display(Name = "Reason")]
        public string reason { get; set; }

        [Display(Name = "Status")]
        public string status { get; set; }

        [Required]
        [Display(Name = "Approver")]
        public string approved_by { get; set; }

        [Required]
        [Display(Name = "Accessible Num")]
        [RegularExpression(@"^(0(\d{3})(\d{3})(\d{2})(\d{2}))$")]
        public string accessible_num { get; set; }

        [Display(Name = "Days")]
        public Nullable<double> days { get; set; }
    }
}