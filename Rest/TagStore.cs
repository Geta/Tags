using EPiServer.Shell.Services.Rest;

namespace Geta.Tags.Rest
{
    [RestStore("tags")]
    public class TagStore : RestControllerBase
    {
        public RestResult Get(string name)
        {
            return Rest(new[]
            {
                "roller",
                "loller",
                "sommer",
                "rommer"
            });
        }
    }
}