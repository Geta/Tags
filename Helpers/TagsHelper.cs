using System;
using EPiServer.Core;
using EPiServer.Web;

namespace Geta.Tags.Helpers
{
    public static class TagsHelper
    {
        public static PageReference GetPageReference(Guid pageGuid)
        {
            var map = PermanentLinkMapStore.Find(pageGuid) as PermanentPageLinkMap;
            return (map != null) ? map.PageReference : PageReference.EmptyReference;
        }
    }
}