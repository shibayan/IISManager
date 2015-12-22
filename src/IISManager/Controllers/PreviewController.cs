using System.Net.Http;
using System.Text;
using System.Web.Http;

using IISManager.Models;

namespace IISManager.Controllers
{
    public class PreviewController : ApiController
    {
        public HttpResponseMessage Post([FromBody]ConfigPostModel model)
        {
            var xdt = AppHost.GenerateXdt(model.NewConfig);

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(xdt, Encoding.UTF8, "text/plain")
            };
        }
    }
}
