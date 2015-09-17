using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Components.DictionaryAdapter.Xml;
using EPiServer;
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
            var groupKey = attributes.FirstOrDefault(a => typeof (TagsGroupKey) == a.GetType()) as TagsGroupKey;
            var cultureSpecific = attributes.FirstOrDefault(a => typeof(CultureSpecificAttribute) == a.GetType()) as CultureSpecificAttribute;
            EditorConfiguration["GroupKey"] = Helpers.TagsHelper.GetGroupKeyFromAttributes(groupKey, cultureSpecific);
        }
    }
}