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
        

        [Authorize(Roles = "admin")]
        public ActionResult CreateUser()
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToRoute("login");

            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult CreateUser([Bind(Include = "id,name,surname,phoneNum,username,password,startDate,role,eligibleDays,daysLeft,isActive")] Employee employee)
        {
            employee.eligibleDays = 0;
            employee.daysLeft = 0;
            employee.isActive = true;

            try
            {
                if (ModelState.IsValid)
                {
                    db.Employee.Add(employee);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error occured.");
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
        public ActionResult EmployeeEdit([Bind(Include = "id,name,surname,phoneNum,username,password,startDate,role,eligibleDays,daysLeft,isActive")] Employee employee)
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