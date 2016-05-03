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
            ClientEditingClass = "geta-tags.TagsSelection";
        }

        public override void ModifyMetadata(EPiServer.Shell.ObjectEditing.ExtendedMetadata metadata,
            IEnumerable<Attribute> attributes)
        {
            base.ModifyMetadata(metadata, attributes);
            var groupKeyAttribute = attributes.FirstOrDefault(a => typeof (TagsGroupKeyAttribute) == a.GetType()) as TagsGroupKeyAttribute;
            var cultureSpecificAttribute = attributes.FirstOrDefault(a => typeof(CultureSpecificAttribute) == a.GetType()) as CultureSpecificAttribute;
            var getaAttribute = attributes.FirstOrDefault(a => typeof(GetaTagsAttribute) == a.GetType()) as GetaTagsAttribute;

            metadata.EditorConfiguration["GroupKey"] = Helpers.TagsHelper.GetGroupKeyFromAttributes(groupKeyAttribute, cultureSpecificAttribute);
            metadata.EditorConfiguration["allowSpaces"] = getaAttribute != null ? getaAttribute.AllowSpaces : false;
            metadata.EditorConfiguration["allowDuplicates"] = getaAttribute != null ? getaAttribute.AllowDuplicates : false;
            metadata.EditorConfiguration["caseSensitive"] = getaAttribute != null ? getaAttribute.CaseSensitive : true;
        }
    }
}