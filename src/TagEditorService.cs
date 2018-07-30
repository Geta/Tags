using System;
using System.Collections.Generic;
using System.Linq;
using Geta.Tags.EditorDescriptors;
using Geta.Tags.Models;

namespace Geta.Tags
{
    public class TagEditorService
    {
        public string EditedTag(string tags,Tag fromRepository,Tag fromUser, GetaTagsAttribute attribute)
        {
            var existingTagName = fromRepository.Name;
            var checksum = tags.LastIndexOf(attribute.SpecialChar) > 0 ? tags.Substring(0, tags.LastIndexOf(attribute.SpecialChar) + 1) : string.Empty;
            IList<string> pageTagList = new List<string>();
            if (!string.IsNullOrEmpty(checksum))
            {
                tags = tags.Replace(checksum, string.Empty);
                var currentSeperator = checksum.Replace(attribute.SpecialChar, string.Empty);
                pageTagList = currentSeperator != attribute.SingleFieldDelimiter
                    ? tags.Split(new[] {currentSeperator}, StringSplitOptions.RemoveEmptyEntries).ToList()
                    : tags.Split(new[] {attribute.SingleFieldDelimiter}, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
            }
            else
            {
                pageTagList = tags.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            
            
            var indexTagToReplace = pageTagList.IndexOf(existingTagName);
            if (indexTagToReplace != -1) 
            {
                pageTagList[indexTagToReplace] = fromUser.Name;
            }
            return $"{attribute.CheckSum}{string.Join(attribute.SingleFieldDelimiter, pageTagList)}";
        }
    }
}