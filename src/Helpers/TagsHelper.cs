using System;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Web;

namespace Geta.Tags.Helpers
{
    public static class TagsHelper
    {
        public static PageReference GetPageReference(Guid pageGuid)
        {
            var map = PermanentLinkMapStore.Find(pageGuid) as PermanentContentLinkMap;
            return (map != null) ? map.ContentReference as PageReference : PageReference.EmptyReference;
        }

        public static bool IsTagProperty(PropertyDefinition propertyDefinition)
        {
            return propertyDefinition != null
                   && propertyDefinition.TemplateHint == "Tags";
        }
    }
}