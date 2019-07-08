using System.ComponentModel.DataAnnotations;
using EPiServer.DataAnnotations;
using Geta.Tags.Attributes;

namespace Geta.Tags.Demo.Models.Pages
{
    /// <summary>
    /// Used primarily for publishing news articles on the website
    /// </summary>
    [SiteContentType(
        GroupName = Global.GroupNames.News,
        GUID = "E92AEE69-F771-40FE-B105-895BD1A9A9E8")]
    [SiteImageUrl(Global.StaticGraphicsFolderPath + "page-type-thumbnail-article.png")]
    public class GetaTagsPage : SitePageData
    {
        [UIHint("Tags")]
        public virtual string Tags1 { get; set; }

        [TagsGroupKey("mykey")]
        [UIHint("Tags")]
        public virtual string Tags2 { get; set; }

        [CultureSpecific]
        [UIHint("Tags")]
        public virtual string Tags3 { get; set; }
    }
}
