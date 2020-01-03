using System.Web.Http;using System.Web.Http.Cors;using System.Web.Http.Dispatcher;

namespace CompilerTool.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
             // Web API configuration and services
             config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
             
             // Web API routes
             config.MapHttpAttributeRoutes();

             config.Services.Replace(typeof(IHttpControllerSelector), new NamespaceHttpControllerSelector(config));

             config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new 
                {   
                    action = "index",
                    id = RouteParameter.Optional,
					namespaceName = new[] {"CompilerTool.WebApi.Controllers"}
					
                }

            );
        }
    }
}