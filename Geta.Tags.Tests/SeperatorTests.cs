using Geta.Tags.EditorDescriptors;
using Geta.Tags.Models;
using Xunit;

namespace Geta.Tags.Tests
{
    public class SeperatorTests
    {
        [Theory]
        [InlineData("abc,dce,efg,hcf")]
        [InlineData("₦;₦abc;dce;efg;hcf")]
        [InlineData("₦|₦abc|dce|efg|hcf")]
        [InlineData("abc,dce,efg,hcf", "abc,dce,efg,hcf", "abc,dce,efg,hcg",  true, ",", true)]
        public void PassingTest(string tags, string fromRepository, string fromUser, bool allowDuplicates, string delimeter, bool allowSpaces)
        {
            var tagEditorService = new TagEditorService();
            var attribute = new GetaTagsAttribute{AllowDuplicates = allowDuplicates, AllowSpaces = allowSpaces, SingleFieldDelimiter = delimeter};
            var editedTag = tagEditorService.EditedTag(tags, new Tag{Name = fromRepository}, new Tag{Name = fromUser}, attribute);
            Assert.NotEqual(tags, editedTag);
        }
    }
}