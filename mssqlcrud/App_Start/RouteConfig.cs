
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MSSQLCRUD
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "EmployeeUpdateProfile",
                url: "employeeUpdateProfile/",
                defaults: new { controller = "Home", action = "employeeUpdateProfile", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ViewProfile",
                url: "profile/",
                defaults: new { controller = "Home", action = "viewprofile", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Update",
                url: "update/",
                defaults: new { controller = "Home", action = "update", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Logout",
                url: "logout/",
                defaults: new { controller = "Home", action = "logout", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Index",
                url: "index/",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Delete",
                url: "delete/",
                defaults: new { controller = "Home", action = "delete", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Register",
                url: "register/",
                defaults: new { controller = "Home", action = "Register", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}