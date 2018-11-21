﻿using System.Collections.Generic;
using EPiServer.Core;
using Geta.Tags.Models;

namespace Geta.Tags.Interfaces
{
    public interface ITagEngine
    {
        IEnumerable<ContentData> GetContentByTag(string tagName);
        IEnumerable<ContentData> GetContentsByTag(Tag tag);
        IEnumerable<ContentData> GetContentsByTag(string tagName, ContentReference rootContentReference);
        IEnumerable<ContentData> GetContentsByTag(Tag tag, ContentReference rootContentReference);
        IEnumerable<ContentReference> GetContentReferencesByTags(string tagNames);
        IEnumerable<ContentReference> GetContentReferencesByTags(IEnumerable<Tag> tags);
        IEnumerable<ContentReference> GetContentReferencesByTags(string tagNames, ContentReference rootContentReference);
        IEnumerable<ContentReference> GetContentReferencesByTags(IEnumerable<Tag> tags, ContentReference rootContentReference);
    }
}