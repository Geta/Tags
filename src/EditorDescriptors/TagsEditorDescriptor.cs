using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.DataAnnotations;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using Geta.Tags.Attributes;

namespace Geta.Tags.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(string), UIHint = "Tags")]
    public class TagsEditorDescriptor : EditorDescriptor
    {
        public TagsEditorDescriptor()
        {
            ClientEditingClass = "geta-tags/TagsSelection";
        }

        public override void ModifyMetadata(EPiServer.Shell.ObjectEditing.ExtendedMetadata metadata,
            IEnumerable<Attribute> attributes)
        {
            var attrs = attributes.ToArray();
            base.ModifyMetadata(metadata, attrs);
            var groupKeyAttribute = attrs.FirstOrDefault(
                a => typeof (TagsGroupKeyAttribute) == a.GetType()) as TagsGroupKeyAttribute;
            var cultureSpecificAttribute = attrs.FirstOrDefault(
                a => typeof(CultureSpecificAttribute) == a.GetType()) as CultureSpecificAttribute;
            var getaAttribute = attrs.FirstOrDefault(
                a => typeof(GetaTagsAttribute) == a.GetType()) as GetaTagsAttribute;

            metadata.EditorConfiguration["GroupKey"] =
                Helpers.TagsHelper.GetGroupKeyFromAttributes(groupKeyAttribute, cultureSpecificAttribute);
            metadata.EditorConfiguration["allowSpaces"] = getaAttribute?.AllowSpaces ?? false;
            metadata.EditorConfiguration["allowDuplicates"] = getaAttribute?.AllowDuplicates ?? false;
            metadata.EditorConfiguration["readOnly"] = getaAttribute?.ReadOnly ?? false;
            metadata.EditorConfiguration["caseSensitive"] = getaAttribute?.CaseSensitive ?? true;
        }
    }
}