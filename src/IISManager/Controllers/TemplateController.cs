using System.Collections.Generic;
using System.Web.Http;

using IISManager.Models;

namespace IISManager.Controllers
{
    public class TemplateController : ApiController
    {
        public IEnumerable<TemplateModel> Get()
        {
            return AppHost.GetTemplates();
        }
    }
}
