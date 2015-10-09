using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class AdminController : Controller
    {
        private CompanyEntities db = new CompanyEntities();

        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToRoute("login");

            var v = from e in db.Employee
                where e.isActive == true
                select e;

            return View(v.ToList());
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Index(string searchString, string date1, string date2)
        {
            var employees = from e in db.Employee
                            select e;

            if (!String.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(s => s.username.StartsWith(searchString)
                                       || s.username.Contains(searchString));

            }

            if (!String.IsNullOrEmpty(date1) && !String.IsNullOrEmpty(date2))
            {

                DateTime start = DateTime.Parse(date1);
                DateTime end = DateTime.Parse(date2);

                employees = from e in db.Employee
                            where e.startDate >= start
                            where e.startDate <= end
                            select e;
            }

            return View(employees.OrderBy(i => i.name).ToList());
        }
        

        [Authorize(Roles = "admin")]
        public ActionResult CreateUser()
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToRoute("login");

            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult CreateUser([Bind(Include = "id,name,surname,phoneNum,username,password,startDate,role,eligibleDays,daysLeft,isActive,email")] Employee employee)
        {
            HomeController hc = new HomeController();
            employee.eligibleDays = hc.CalcEligibileDays(employee.startDate);
            employee.isActive = true;

            try
            {
                if (employee.daysLeft == null)
                {
                    employee.daysLeft = employee.eligibleDays;
                }

                var unames = (from e in db.Employee
                             where e.username == employee.username
                             select e).FirstOrDefault();

                if (unames != null)
                {
                    ModelState.AddModelError("","The username is in use.");
                }

                
                if (ModelState.IsValid)
                {
                    db.Employee.Add(employee);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error while creating user. Please check the info you typed.");
            }
            
            return View();
        }


        [Authorize(Roles = "admin")]
        public ActionResult DeleteUser(int? id)
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToRoute("login");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Employee employee = db.Employee.Find(id);

            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            Employee employee = db.Employee.Find(id);
            employee.isActive = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "admin")]
        public ActionResult AdminSettings()
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToRoute("login");

            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult EmployeeEdit(int id)
        {
            Employee employee = db.Employee.Find(id);

            if (User.Identity.IsAuthenticated == false)
                return RedirectToRoute("login");

            return View(employee);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult EmployeeEdit([Bind(Include = "id,name,surname,phoneNum,username,password,startDate,role,eligibleDays,daysLeft,isActive,email")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employee);
        }
    }
}