using System.Web.Http;

using IISManager.Models;

namespace IISManager.Controllers
{
    public class XdtController : ApiController
    {
        public IHttpActionResult Post([FromBody]ConfigPostModel model)
        {
            var xdt = AppHost.GenerateXdt(AppHost.GetCurrentConfig(), model.NewConfig);

            AppHost.SaveAppHostXdt(xdt);

            return Ok();
        }
    }
}
