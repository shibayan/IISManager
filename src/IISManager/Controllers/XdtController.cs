using System.Net.Http;
using System.Text;
using System.Web.Http;

using IISManager.Models;

namespace IISManager.Controllers
{
    public class XdtController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var currentXdt = AppHost.GetCurrentXdt();

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(currentXdt, Encoding.UTF8, "text/plain")
            };
        }

        public IHttpActionResult Post([FromBody]XdtPostModel model)
        {
            AppHost.SaveFromXdt(model.NewXdt);

            return Ok();
        }

        [Route("api/xdt/preview")]
        [HttpPost]
        public HttpResponseMessage Preview([FromBody]XdtPostModel model)
        {
            var config = AppHost.GenerateConfig(model.NewXdt);

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(config, Encoding.UTF8, "text/plain")
            };
        }
    }
}
