using EPiServer.Core;
using EPiServer.PlugIn;

namespace Geta.Tags.SpecializedProperties
{
    [PageDefinitionTypePlugIn]
    public class PropertyTags : PropertyLongString
    {
        public override IPropertyControl CreatePropertyControl()
        {
            return new PropertyTagsControl();
        }
    }
}