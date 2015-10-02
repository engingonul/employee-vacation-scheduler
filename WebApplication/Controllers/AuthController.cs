using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;


namespace WebApplication.Controllers
{
    public class AuthController : Controller
    {
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

                    if (v != null)
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

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToRoute("login");   
        }

    }
}