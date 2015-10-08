using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private CompanyEntities db = new CompanyEntities();

        // GET: Home
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToRoute("login");

            return View();
        }


        // GET: User/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "id,name,surname,phoneNum,username,password,startDate,role,eligibleDays,daysLeft,isActive")] Employee employee)
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToRoute("login");
            

            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("PersonalInfo");
            }

            return View(employee);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToRoute("login");

            Employee employee = db.Employee.Find(id);
            db.Employee.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult NewRequest()
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToRoute("login");

            ViewBag.Item = new SelectList(db.VacationTypes, "item", "item");

            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("NewRequest")]
        [ValidateAntiForgeryToken]
        public ActionResult NewRequest(
            [Bind(Include = "emp_id,type,start_date,end_date,reason,accessible_num,status,approved_by,days")] Vacation vacation, string halfdays)
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToRoute("login");
            try
                {
                    ViewBag.Item = new SelectList(db.VacationTypes, "item", "item", vacation.type);
            
                    if (halfdays == null)
                {
                    halfdays = "0";
                }

                Regex regex = new Regex("^[0-9]*$");
                if (!regex.IsMatch(halfdays))
                {
                    ModelState.AddModelError("", "Only numbers allowed in 'Half Days' area.");
                }


                if (GetWorkDays(vacation.start_date, vacation.end_date) < Convert.ToInt32(halfdays))
                {
                    ModelState.AddModelError("","Half days can not be more than vacation days. Selected vacation days: " + GetWorkDays(vacation.start_date, vacation.end_date) );
                }

                vacation.emp_id = Convert.ToInt32(Session["LoggedUserID"]);
                vacation.status = "Pending";
                vacation.days = GetWorkDays(vacation.start_date, vacation.end_date) - (Convert.ToInt32(halfdays)*0.5);

                Employee employee = db.Employee.Find(vacation.emp_id);
            
                //vacation days validation
                if (vacation.start_date < DateTime.Today.Date || vacation.start_date < DateTime.Today.Date || vacation.start_date > vacation.end_date)
                {
                    ModelState.AddModelError("", "Invalid vacation dates.");
                }

                if (employee.daysLeft < vacation.days)
                {
                    if (employee.eligibleDays == 0)
                    {
                        if (employee.daysLeft > -5)
                        {
                            if (vacation.days > 5)
                                ModelState.AddModelError("", "Freshmen can only take 5 days off.");
                        }
                    }
                    else
                        ModelState.AddModelError("", "You don't have enough days.");
                }

            
                    if (ModelState.IsValid)
                    {
                        db.Vacation.Add(vacation);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
            }
            catch (Exception)
            {
                ModelState.AddModelError("","Error occured. Please check the info you typed.");
            }
            
            return View();
        }

        public ActionResult PersonalInfo()
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToRoute("login");

            return View(db.Employee.ToList());
        }

        public ActionResult Vacations()
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToRoute("login");

            return View(db.Vacation.OrderByDescending(x => x.id).ToList());
        }

        public ActionResult VacationsAwaiting(int? id)
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToRoute("login");


            var model = from e in db.Employee
                join v in db.Vacation on e.id equals v.emp_id
                where v.status == "Pending"
                orderby v.id descending
                select new MultipleViewModel
                {Employee = e, Vacation = v};

            return View(model.ToList());
        }

        public double CalcEligibileDays(DateTime jobStart)
        {
            double workDays = (DateTime.Now.Date - jobStart).TotalDays;
            double workYears = workDays/365;
            double eligibleDays = 0;
            double vacation = 0;

            if (workYears < 1)
            {
                eligibleDays = 0;
                vacation = eligibleDays*workYears;
            }
            else if (workYears >= 1 && workYears < 6)
            {
                eligibleDays = 15;
                vacation = eligibleDays*(int) workYears;
            }
            else if (workYears >= 6)
            {
                eligibleDays = 20;
                vacation = (eligibleDays*(int) (workYears - 1)) - 5;
            }

            return vacation;
        }

        public void RefreshInfo(int id)
        {
            Employee employee = db.Employee.Find(id);
            double? eDaysBefore = employee.eligibleDays;
            
            db.Entry(employee).State = EntityState.Modified;
            employee.eligibleDays = CalcEligibileDays(employee.startDate);
            employee.daysLeft = employee.daysLeft + (employee.eligibleDays - eDaysBefore);
            db.SaveChanges();
        }

        public static double GetWorkDays(DateTime startD, DateTime endD)
        {
            double calcWorkDays = 1 +
                                  ((endD - startD).TotalDays*5 - (startD.DayOfWeek - endD.DayOfWeek)*2)/7;

            if ((int) endD.DayOfWeek == 6)
                calcWorkDays--;
            if ((int) startD.DayOfWeek == 0)
                calcWorkDays--;

            return calcWorkDays;
        }
        
        public void AcceptVacation(int id)
        {
            Vacation vacation = db.Vacation.Find(id);
            Employee employee = db.Employee.Find(vacation.emp_id);
            
            db.Entry(vacation).State = EntityState.Modified;
            vacation.status = "Approved";
            db.SaveChanges();

            db.Entry(employee).State = EntityState.Modified;
            employee.eligibleDays = CalcEligibileDays(employee.startDate);
            employee.daysLeft = employee.daysLeft - vacation.days; 
            db.SaveChanges();
        }

        public void DenyVacation(int id)
        {
            Vacation vacation = db.Vacation.Find(id);

            db.Entry(vacation).State = EntityState.Modified;
            vacation.status = "Denied";
            db.SaveChanges();
        }

        public ActionResult EmployeeInfo()
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToRoute("login");

            IEnumerable<SelectListItem> names = db.Employee
              .Select(c => new SelectListItem
              {
                  Value = c.id.ToString(),
                  Text = c.name + " " + c.surname
              });
            ViewBag.Employee = names;
            

            var model = from e in db.Employee
                        orderby e.name
                        select e;

            return View(model);
        }

        [HttpPost]
        public ActionResult EmployeeInfo(string searchString, string LeftDays, string date1, string date2)
        {
            var employees = from e in db.Employee
                            select e;
            
            if (!String.IsNullOrEmpty(LeftDays))
            {
                if (LeftDays == "0")
                {
                    employees = from e in db.Employee
                        where e.daysLeft <= 5
                        select e;
                }
                else if (LeftDays == "1")
                {
                    employees = from e in db.Employee
                                where e.daysLeft > 5
                                where e.daysLeft <= 15
                                select e;
                }
                else if (LeftDays == "2")
                {
                    employees = from e in db.Employee
                                where e.daysLeft > 15
                                where e.daysLeft <= 30
                                select e;
                }
                else if (LeftDays == "3")
                {
                    employees = from e in db.Employee
                                where e.daysLeft > 30
                                select e;
                }
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(s => s.name.StartsWith(searchString)
                                       || s.name.Contains(searchString));
                
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

            return View(employees.OrderBy(i=>i.name).ToList());
        }

        public ActionResult EmployeeReview(int? id)
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToRoute("login");
           

            var model = from e in db.Employee
                        join v in db.Vacation on e.id equals v.emp_id
                        where e.id == id
                        orderby v.start_date descending
                        select new MultipleViewModel
                        { Employee = e, Vacation = v };

            return View(model.ToList());
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}