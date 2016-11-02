using System;
using System.Linq;
using System.Web.Mvc;
using EPiServer.DataAnnotations;
using EPiServer.Shell;
using EPiServer.Shell.ObjectEditing;
using Geta.Tags.Attributes;

namespace Geta.Tags.EditorDescriptors
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GetaTagsAttribute : Attribute, IMetadataAware
    {
        public bool AllowSpaces { get; set; }
        public bool CaseSensitive { get; set; }
        public bool AllowDuplicates { get; set; }

        public bool ReadOnly { get; set; }

        public GetaTagsAttribute() {
            AllowDuplicates = false;
            AllowSpaces = false;
            CaseSensitive = true;
            ReadOnly = false;
        }

        public virtual void OnMetadataCreated(ModelMetadata metadata)
        {
            var extendedMetadata = metadata as ExtendedMetadata;

            if (extendedMetadata == null)
            {
                return;
            }

            var groupKeyAttribute = extendedMetadata.Attributes.FirstOrDefault(a => typeof(TagsGroupKeyAttribute) == a.GetType()) as TagsGroupKeyAttribute;
            var cultureSpecificAttribute = extendedMetadata.Attributes.FirstOrDefault(a => typeof(CultureSpecificAttribute) == a.GetType()) as CultureSpecificAttribute;

            extendedMetadata.ClientEditingClass = "geta-tags/TagsSelection";
            extendedMetadata.CustomEditorSettings["uiType"] = extendedMetadata.ClientEditingClass;
            extendedMetadata.CustomEditorSettings["uiWrapperType"] = UiWrapperType.Floating;
            extendedMetadata.EditorConfiguration["GroupKey"] = Helpers.TagsHelper.GetGroupKeyFromAttributes(groupKeyAttribute, cultureSpecificAttribute);
            extendedMetadata.EditorConfiguration["allowSpaces"] = this.AllowSpaces;
            extendedMetadata.EditorConfiguration["allowDuplicates"] = this.AllowDuplicates;
            extendedMetadata.EditorConfiguration["caseSensitive "] = this.CaseSensitive;
            extendedMetadata.EditorConfiguration["readOnly "] = this.ReadOnly;
        }
    }
}