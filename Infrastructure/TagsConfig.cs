using System.Web.Http;

namespace Geta.Tags.Infrastructure
{
    public static class TagsConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "TagsApi",
                routeTemplate: "api/tags/{id}",
                defaults: new { Controller = "Tags", Id = RouteParameter.Optional }
                );
        }
    }
}