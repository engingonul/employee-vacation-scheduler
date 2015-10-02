using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Login", "", new {controller = "Auth", action = "Login"});

            routes.MapRoute("Logout", "logout", new { controller = "Auth", action = "Logout" });

            routes.MapRoute("Home", "home", new { controller = "Home", action = "Index" });

            routes.MapRoute("Edit", "edit", new {controller = "Home", action="Edit"});

            routes.MapRoute("Delete", "delete", new { controller = "Home", action = "Delete" });

            routes.MapRoute("Create", "create", new { controller = "Home", action = "Create" });

            routes.MapRoute("Details", "details", new { controller = "Home", action = "Details" });

            routes.MapRoute("PersonalInfo", "personalInfo", new { controller = "Home", action = "PersonalInfo" });

            routes.MapRoute("NewRequest", "newrequest", new { controller = "Home", action = "NewRequest" });

            routes.MapRoute("Vacations", "vacations", new { controller = "Home", action = "Vacations" });

            routes.MapRoute("VacationsAwaiting", "vacationsAwaiting", new { controller = "Home", action = "VacationsAwaiting" });

            routes.MapRoute("AcceptVacation", "AcceptVacation", new { controller = "Home", action = "AcceptVacation" });

            routes.MapRoute("DenyVacation", "DenyVacation", new { controller = "Home", action = "DenyVacation" });

            routes.MapRoute("CalcEligibileDays", "calcEligibileDays", new { controller = "Home", action = "CalcEligibileDays" });

            routes.MapRoute("RefreshInfo", "refreshInfo", new { controller = "Home", action = "RefreshInfo" });

            routes.MapRoute("EmployeeInfo", "employeeInfo", new { controller = "Home", action = "EmployeeInfo" });

            routes.MapRoute("EmployeeReview", "employeeReview", new { controller = "Home", action = "EmployeeReview" });
        }
    }
}
