using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using DataProvider.Objects;

namespace DataProvider.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "dataapi",
                routeTemplate: "data/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                ).RouteHandler = new SessionHttpControllerRouteHandler();

            routes.MapHttpRoute(
               name: "odata",
               routeTemplate: "odata/{controller}/{id}",
               defaults: new { id = RouteParameter.Optional }
               ).RouteHandler = new SessionHttpControllerRouteHandler();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}