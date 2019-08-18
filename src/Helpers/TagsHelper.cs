// Copyright (c) Geta Digital. All rights reserved.
// Licensed under MIT. See the LICENSE file in the project root for more information

using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Globalization;
using EPiServer.Web;
using Geta.Tags.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Geta.Tags.Helpers
{
    public static class TagsHelper
    {
        public static string GetGroupKeyFromAttributes(
            TagsGroupKeyAttribute groupKeyAttribute, CultureSpecificAttribute cultureSpecificAttribute, IContent content)
        {
            var groupKey = string.Empty;

            if (groupKeyAttribute == null && cultureSpecificAttribute == null && content is ILocalizable localizableContent)
            {
                groupKey += localizableContent.MasterLanguage;
            }

            if (groupKeyAttribute != null)
            {
                groupKey += groupKeyAttribute.Key;
            }

            if (cultureSpecificAttribute != null && cultureSpecificAttribute.IsCultureSpecific)
            {
                groupKey += ContentLanguage.PreferredCulture ?? CultureInfo.CurrentCulture;
            }

            return groupKey;
        }

        public static ContentReference GetContentReference(Guid contentGuid)
        {
            return PermanentLinkUtility.FindContentReference(contentGuid) ?? ContentReference.EmptyReference;
        }

        public static IEnumerable<ContentReference> GetContentReferences(List<Guid> contentLinks)
        {
            foreach (var contentLink in contentLinks)
            {
                var reference = GetContentReference(contentLink);

                if (!ContentReference.IsNullOrEmpty(reference))
                {
                    yield return reference;
                }
            }
        }

        public static bool IsTagProperty(PropertyDefinition propertyDefinition)
        {
            return propertyDefinition != null 
                   && propertyDefinition.TemplateHint == "Tags";
        }
    }
}
