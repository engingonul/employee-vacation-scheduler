using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace WebApplication.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/styles")
                .Include("~/Content/bootstrap.css")
                .Include("~/Content/Styles/site.css")
                .Include("~/Content/Styles/login.css")
                .Include("~/Content/Styles/error.css")
                .Include("~/Content/Styles/Home.css"));

            bundles.Add(new ScriptBundle("~/scripts")
                .Include("~/Scripts/jquery-2.1.4.js")
                .Include("~/Scripts/jquery.validate.js")
                .Include("~/Scripts/jquery.validate.unobtrusive.js")
                .Include("~/Scripts/bootstrap.js"));
        }
    }
}