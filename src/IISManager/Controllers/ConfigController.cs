using System.Net.Http;
using System.Text;
using System.Web.Http;

using IISManager.Models;

namespace IISManager.Controllers
{
    public class ConfigController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var currentConfig = AppHost.GetCurrentConfig();

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(currentConfig, Encoding.UTF8, "text/plain")
            };
        }

        public IHttpActionResult Post([FromBody]ConfigPostModel model)
        {
            AppHost.SaveFromConfig(model.NewConfig);

            return Ok();
        }

        [Route("api/config/preview")]
        [HttpPost]
        public HttpResponseMessage Preview([FromBody]ConfigPostModel model)
        {
            var xdt = AppHost.GenerateXdt(model.NewConfig);

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(xdt, Encoding.UTF8, "text/plain")
            };
        }
    }
}
