using System.Web.Mvc;
using System.Web.Routing;

namespace CompilerTool.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",defaults: new { controller = "OrderItem", action = "Index", id = UrlParameter.Optional },
				namespaces:new[]  {"CompilerTool.Web.Controllers"});
				
        }
    }
}