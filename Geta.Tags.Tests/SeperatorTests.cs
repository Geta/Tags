using Geta.Tags.EditorDescriptors;
using Geta.Tags.Models;
using Xunit;

namespace Geta.Tags.Tests
{
    public class SeperatorTests
    {
        [Theory]
        [InlineData("abc,dce,efg,hcf", "hcf", "hcg", true, ",", true)]
        [InlineData("₦;₦abc;dce;efg;hcf", "hcf", "hdf", true, "|", true)]
        [InlineData("₦|₦abc|dce|efg|hcf", "hcf", "hhg", true, "|||", true)]
        [InlineData("abc,dce,efg,hcf", "hcf", "hcg",  true, ",", true)]
        public void SeperatorMigrationTest(string tags, 
            string fromRepository, 
            string fromUser, 
            bool allowDuplicates, 
            string delimeter, 
            bool allowSpaces)
        {
            var tagEditorService = new TagEditorService();
            var attribute = new GetaTagsAttribute
            {
                AllowDuplicates = allowDuplicates,
                AllowSpaces = allowSpaces,
                SingleFieldDelimiter = delimeter
            };
            var editedTag = tagEditorService.EditedTag(tags, new Tag {Name = fromRepository}, new Tag {Name = fromUser},
                attribute);
            Assert.NotEqual(tags, editedTag);
        }
    }
}