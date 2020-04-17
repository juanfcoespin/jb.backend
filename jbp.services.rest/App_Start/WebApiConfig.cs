using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;


using System.Web.Http.Cors;

namespace jbp.services.rest
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            var is64 = Environment.Is64BitProcess;
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //Estas dos lineas de código permiten los post, put y delete
            EnableCorsAttribute cors = new EnableCorsAttribute("*", "*", "*");
            //cors.o = new string[] { ""};
            config.EnableCors(cors);
        }
    }
}
