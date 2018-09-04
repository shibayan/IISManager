using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace IISManager
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            MvcHandler.DisableMvcResponseHeader = true;
        }
    }
}
