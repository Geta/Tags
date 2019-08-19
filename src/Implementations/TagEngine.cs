// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Geta.Tags.Interfaces;
using Geta.Tags.Models;
using System;
using Geta.Tags.Helpers;

namespace Geta.Tags.Implementations
{
    [ServiceConfiguration(typeof(ITagEngine))]
    public class TagEngine : ITagEngine
    {
        private readonly ITagService _tagService;
        private readonly IContentLoader _contentLoader;

        public TagEngine(ITagService tagService, IContentLoader contentLoader)
        {
            _tagService = tagService;
            _contentLoader = contentLoader;
        }

        public IEnumerable<ContentData> GetContentByTag(string tagName)
        {
            return !string.IsNullOrEmpty(tagName)
                ? _tagService.GetTagsByName(tagName).SelectMany(GetContentsByTag)
                : Enumerable.Empty<ContentData>();
        }

        public IEnumerable<ContentData> GetContentsByTag(Tag tag)
        {
            if (tag == null)
            {
                return Enumerable.Empty<ContentData>();
            }

            var contentLinks = new List<Guid>();

            if (tag.PermanentLinks == null)
            {
                var tempTerm = _tagService.GetTagByNameAndGroup(tag.Name, tag.GroupKey);

                if (tempTerm != null)
                {
                    contentLinks = tempTerm.PermanentLinks.ToList();
                }
            }
            else
            {
                contentLinks = tag.PermanentLinks.ToList();
            }

            var contentReferences = TagsHelper.GetContentReferences(contentLinks);

            return contentReferences
                .Select(contentLink => _contentLoader.Get<ContentData>(contentLink))
                .ToList();
        }

        public IEnumerable<ContentData> GetContentsByTag(string tagName, ContentReference rootContentReference)
        {
            return !string.IsNullOrEmpty(tagName)
                ? _tagService.GetTagsByName(tagName).SelectMany(t => GetContentsByTag(t, rootContentReference))
                : Enumerable.Empty<ContentData>();
        }

        public IEnumerable<ContentData> GetContentsByTag(Tag tag, ContentReference rootContentReference)
        {
            if (tag?.PermanentLinks == null)
            {
                return Enumerable.Empty<ContentData>();
            }

            var descendantContentReferences = _contentLoader.GetDescendents(rootContentReference)?.ToArray();

            if (descendantContentReferences == null || !descendantContentReferences.Any())
            {
                return Enumerable.Empty<ContentData>();
            }

            return tag
                .PermanentLinks
                .Select(TagsHelper.GetContentReference)
                .Where(contentLink =>
                    descendantContentReferences.FirstOrDefault(p => p.ID == contentLink.ID) != null)
                .Select(contentReference => _contentLoader.Get<PageData>(contentReference))
                .Cast<ContentData>()
                .ToList();
        }

        public IEnumerable<ContentReference> GetContentReferencesByTags(string tagNames)
        {
            if (string.IsNullOrEmpty(tagNames))
            {
                return Enumerable.Empty<ContentReference>();
            }

            var tags = GetTags(tagNames);
            return GetContentReferencesByTags(tags);
        }

        private List<Tag> GetTags(string tagNames)
        {
            return tagNames
                .Split(',')
                .SelectMany(tagName => _tagService.GetTagsByName(tagName))
                .ToList();
        }

        public IEnumerable<ContentReference> GetContentReferencesByTags(IEnumerable<Tag> tags)
        {
            var matches = new Dictionary<ContentReference, int>();

            foreach (var tag in tags)
            {
                if (tag?.PermanentLinks == null) continue;

                foreach (var contentGuid in tag.PermanentLinks)
                {
                    var contentReference = TagsHelper.GetContentReference(contentGuid);

                    if (matches.ContainsKey(contentReference))
                    {
                        matches[contentReference] += 1;
                    }
                    else
                    {
                        matches.Add(contentReference, 1);
                    }
                }
            }

            matches = matches.OrderByDescending(t => t.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            return matches.Keys;
        }

        public IEnumerable<ContentReference> GetContentReferencesByTags(string tagNames, ContentReference rootContentReference)
        {
            if (string.IsNullOrEmpty(tagNames))
            {
                return Enumerable.Empty<ContentReference>();
            }
            var tags = GetTags(tagNames);
            return GetContentReferencesByTags(tags, rootContentReference);
        }

        public IEnumerable<ContentReference> GetContentReferencesByTags(IEnumerable<Tag> tags, ContentReference rootContentReference)
        {
            if (tags == null || ContentReference.IsNullOrEmpty(rootContentReference))
            {
                return Enumerable.Empty<ContentReference>();
            }

            var descendantPageReferences = _contentLoader.GetDescendents(rootContentReference)?.ToArray();

            if (descendantPageReferences == null || !descendantPageReferences.Any())
            {
                return Enumerable.Empty<ContentReference>();
            }

            var matches = new Dictionary<ContentReference, int>();

            foreach (var tag in tags)
            {
                if (tag?.PermanentLinks == null)
                {
                    continue;
                }

                foreach (var contentGuid in tag.PermanentLinks)
                {
                    var contentReference = TagsHelper.GetContentReference(contentGuid);

                    if (descendantPageReferences.FirstOrDefault(p => p.ID == contentReference.ID) == null) continue;

                    if (matches.ContainsKey(contentReference))
                    {
                        matches[contentReference] += 1;
                    }
                    else
                    {
                        matches.Add(contentReference, 1);
                    }
                }
            }

            matches = matches.OrderByDescending(t => t.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            return matches.Keys;
        }
    }
}