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
            metadata.EditorConfiguration["GroupKey"] = Helpers.TagsHelper.GetGroupKeyFromAttributes(groupKeyAttribute, cultureSpecificAttribute);
            metadata.EditorConfiguration["allowSpaces"] = false;
            metadata.EditorConfiguration["allowDuplicates"] = false;
            metadata.EditorConfiguration["caseSensitive"] = true;
        }
    }
}