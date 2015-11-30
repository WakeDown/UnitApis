using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.WebHost;
using DataProvider.Models;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using DataProvider.Models.Stuff;
using DataProvider.Objects;

namespace DataProvider.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var httpControllerRouteHandler = typeof(HttpControllerRouteHandler).GetField("_instance",
                            BindingFlags.Static |
                            BindingFlags.NonPublic);

            if (httpControllerRouteHandler != null)
            {
                httpControllerRouteHandler.SetValue(null,
                    new Lazy<HttpControllerRouteHandler>(() => new SessionHttpControllerRouteHandler(), true));
            }

            config.Filters.Add(new AuthorizeAttribute());
            //config.Filters.Add(new AuthorizeAdAttribute()); 

            config.EnableQuerySupport();
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "dataapi",
                routeTemplate: "data/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "odata",
                routeTemplate: "odata/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        
    }
}
