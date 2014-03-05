using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace Geta.Tags.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(string), UIHint = "Tags")]
    public class TagsEditorSelectionEditorDescriptor : EditorDescriptor
    {
        public TagsEditorSelectionEditorDescriptor()
        {
            ClientEditingClass = "geta.editors.TagsSelection";
        }
    }
}