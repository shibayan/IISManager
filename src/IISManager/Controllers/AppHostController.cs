using System.Net.Http;
using System.Text;
using System.Web.Http;

using IISManager.Models;

namespace IISManager.Controllers
{
    public class AppHostController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var currentConfig = AppHost.GetCurrentConfig();

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(currentConfig, Encoding.UTF8, "text/plain")
            };
        }
    }
}
