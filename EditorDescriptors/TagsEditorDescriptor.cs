using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace Geta.Tags.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(string), UIHint = "Tags")]
    public class TagsEditorDescriptor : EditorDescriptor
    {
        public TagsEditorDescriptor()
        {
            ClientEditingClass = "geta-tags.TagsSelection";
        }
    }
}