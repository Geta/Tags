using EPiServer.DataAnnotations;

namespace Geta.Tags.Demo.Models
{
    /// <summary>
    /// Attribute to set the default thumbnail for site page and block types
    /// </summary>
    public class SiteImageUrl : ImageUrlAttribute
    {
        /// <summary>
        /// The parameterless constructor will initialize a SiteImageUrl attribute with a default thumbnail
        /// </summary>
        public SiteImageUrl() : base("~/Static/gfx/page-type-thumbnail.png")
        {

        }

        public SiteImageUrl(string path) : base(path)
        {

        }
    }
}
