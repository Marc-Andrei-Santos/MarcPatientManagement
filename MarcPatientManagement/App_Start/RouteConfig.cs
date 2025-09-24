using System.Web.Mvc;
using System.Web.Routing;

namespace AL
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Enable attribute routing
            routes.MapMvcAttributeRoutes();

            // Default route
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Medication", action = "Index", id = UrlParameter.Optional }
            ).DataTokens["area"] = "Medication";

            // Catch-all for 404
            routes.MapRoute(
                name: "NotFound",
                url: "{*url}",
                defaults: new { controller = "Error", action = "NotFound" }
            );
        }
    }
}
