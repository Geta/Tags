using EPiServer.Cms.Shell.Extensions;
using EPiServer.DataAnnotations;
using EPiServer.Shell;
using EPiServer.Shell.ObjectEditing;
using Geta.Tags.Attributes;
using Geta.Tags.Helpers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Geta.Tags.EditorDescriptors
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GetaTagsAttribute : Attribute, IMetadataAware
    {
        public bool AllowSpaces { get; set; }
        public bool CaseSensitive { get; set; }
        public bool AllowDuplicates { get; set; }
        public int TagLimit { get; set; }

        public bool ReadOnly { get; set; }

        public GetaTagsAttribute()
        {
            AllowDuplicates = false;
            AllowSpaces = false;
            CaseSensitive = true;
            ReadOnly = false;
            TagLimit = -1;
        }

        public virtual void OnMetadataCreated(ModelMetadata metadata)
        {
            var extendedMetadata = metadata as ExtendedMetadata;

            if (extendedMetadata == null)
            {
                return;
            }

            var groupKeyAttribute = extendedMetadata
                .Attributes
                .FirstOrDefault(a => typeof(TagsGroupKeyAttribute) == a.GetType()) as TagsGroupKeyAttribute;
            var cultureSpecificAttribute = extendedMetadata
                .Attributes
                .FirstOrDefault(a => typeof(CultureSpecificAttribute) == a.GetType()) as CultureSpecificAttribute;
            var ownerContent = extendedMetadata.FindOwnerContent();

            extendedMetadata.ClientEditingClass = "geta-tags/TagsSelection";
            extendedMetadata.CustomEditorSettings["uiType"] = extendedMetadata.ClientEditingClass;
            extendedMetadata.CustomEditorSettings["uiWrapperType"] = UiWrapperType.Floating;
            extendedMetadata.EditorConfiguration["GroupKey"] =
                TagsHelper.GetGroupKeyFromAttributes(groupKeyAttribute, cultureSpecificAttribute, ownerContent);
            extendedMetadata.EditorConfiguration["allowSpaces"] = AllowSpaces;
            extendedMetadata.EditorConfiguration["allowDuplicates"] = AllowDuplicates;
            extendedMetadata.EditorConfiguration["caseSensitive "] = CaseSensitive;
            extendedMetadata.EditorConfiguration["readOnly "] = ReadOnly;
            extendedMetadata.EditorConfiguration["tagLimit"] = TagLimit;
        }
    }
}