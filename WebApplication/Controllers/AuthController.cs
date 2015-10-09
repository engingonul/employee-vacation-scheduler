using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using Postal;
using WebApplication.Models;
using System.Net;
using System.Net.Mail;
using System.Web.WebSockets;


namespace WebApplication.Controllers
{
    public class AuthController : Controller
    {
        private CompanyEntities db = new CompanyEntities();

        // GET: Auth
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Employee form, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);
                

                using (CompanyEntities ce = new CompanyEntities())
                {
                    var v = ce.Employee.Where(a => a.username.Equals(form.username) && a.password.Equals(form.password)).FirstOrDefault();

                    var n = ce.Admin.Where(x => x.username.Equals(form.username) && x.password.Equals(form.password))
                            .FirstOrDefault();

                    if (n != null)
                    {
                        FormsAuthentication.SetAuthCookie(n.username, true);

                        return RedirectToRoute("index");
                    }

                    if (v != null && v.isActive == true)
                    {
                        FormsAuthentication.SetAuthCookie(v.username, true);

                        Session["LoggedUserID"] = v.id;
                        Session["LoggedUserDaysLeft"] = v.daysLeft;
                        Session["LoggedUserFullname"] = v.name.ToString() + " " + v.surname.ToString();
                        

                        if (v.role == true)  //if true then redirect to hr login
                        {
                            Session["LoggedUserRole"] = "hr";
                            return RedirectToRoute("home");
                        }
                        else
                        {
                            Session["LoggedUserRole"] = "employee";
                            return RedirectToRoute("home");
                        }
                        
                    }
                    else
                    {
                        ModelState.AddModelError("", "The username or password is incorrect.");
                    }
                }
            }
            
            return View(form);

        }

        public ActionResult PasswordReset()
        {

            return View();
        }

        [HttpPost]
        public ActionResult PasswordReset(string username, string name, string surname, string phoneNum, string userEmail)
        {
            try
            {
                var query = (from e in db.Employee
                             where e.username == username
                             where e.name == name
                             where e.surname == surname
                             where e.phoneNum == phoneNum
                             where e.email == userEmail
                             select e).FirstOrDefault();

                string query2 = (from e in db.Employee
                                 where e.username == username
                                 where e.name == name
                                 where e.surname == surname
                                 where e.phoneNum == phoneNum
                                 where e.email == userEmail
                                 select e.password).FirstOrDefault();

                if (query != null)
                {
                        MailMessage msg = new MailMessage();
                        msg.From = new MailAddress("egonul8@gmail.com");
                        msg.To.Add(userEmail);
                        msg.Subject = "Password Info";
                        msg.Body = "Your password is : " + query2;
                        SmtpClient sc = new SmtpClient("smtp.gmail.com");
                        sc.Port = 587;
                        sc.Credentials = new NetworkCredential("egonul8@gmail.com", "CBR600RR");
                        sc.EnableSsl = true;
                        sc.Send(msg);
                        ViewBag.mailMessage = "e-mail has been sent. Please check your inbox.";
                        
                }
                else
                {
                        ModelState.AddModelError("", "The data provided does not match the username. All fields must be filled.");
                   
                }
            }
            catch (Exception ex)
            {
                ViewBag.mailMessage = ex.Message;
            }
            

            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToRoute("login");   
        }
        
    }
}