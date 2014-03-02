using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Geta.Tags.Api
{
    public class TagsController : ApiController
    {
        public HttpResponseMessage Get(string name)
        {
            return Request.CreateResponse(HttpStatusCode.OK, new[]
            {
                new {name = "roller", id = "1"},
                new {name = "loller", id = "2"},
                new {name = "sommer", id = "3"},
                new {name = "rommer", id = "4"}
            });
        }
    }
}