using System.Web.Http;

using IISManager.Models;

namespace IISManager.Controllers
{
    public class XdtController : ApiController
    {
        public IHttpActionResult Post([FromBody]ConfigPostModel model)
        {
            AppHost.ApplyConfig(model.NewConfig);

            return Ok();
        }
    }
}
