using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class MultipleViewModel
    {
        public Employee Employee { get; set; }
        public Vacation Vacation { get; set; }
        public VacationTypes VacationTypes { get; set; }

    }
}