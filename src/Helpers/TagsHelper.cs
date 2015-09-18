using System;
using System.Globalization;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using Geta.Tags.Attributes;

namespace Geta.Tags.Helpers
{
    public static class TagsHelper
    {
        public static string GetGroupKeyFromAttributes(TagsGroupKeyAttribute groupKeyAttribute, CultureSpecificAttribute cultureSpecificAttribute)
        {
            string groupKey = "";
            if (groupKeyAttribute != null)
            {
                groupKey += groupKeyAttribute.Key;
            }
            if ((cultureSpecificAttribute != null) && (cultureSpecificAttribute.IsCultureSpecific))
            {
                groupKey += CultureInfo.CurrentCulture;
            }
            return groupKey;
        }
        [Obsolete("Use GetContentReference instead")]
        public static PageReference GetPageReference(Guid pageGuid)
        {
            var map = PermanentLinkMapStore.Find(pageGuid) as PermanentContentLinkMap;
            return (map != null) ? map.ContentReference as PageReference : PageReference.EmptyReference;
        }

        public static ContentReference GetContentReference(Guid contentGuid)
        {
            return PermanentLinkUtility.FindContentReference(contentGuid) ?? ContentReference.EmptyReference;
        }

        public static bool IsTagProperty(PropertyDefinition propertyDefinition)
        {
            return propertyDefinition != null
                   && propertyDefinition.TemplateHint == "Tags";
        }
    }
}