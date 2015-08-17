using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DataProvider.Models;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using DataProvider.Models.Stuff;

namespace DataProvider.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filters.Add(new AuthorizeAttribute());

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
