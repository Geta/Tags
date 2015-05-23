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
            this._tagService = tagService;
            this._contentLoader = contentLoader;
        }

        [Obsolete("Use GetContentByTag instead.")]
        public PageDataCollection GetPagesByTag(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                return null;
            }

            return this.GetPagesByTag(this._tagService.GetTagByName(tagName));
        }

        [Obsolete("Use GetContentsByTag instead.")]
        public PageDataCollection GetPagesByTag(Tag tag)
        {
            if (tag == null)
            {
                return null;
            }

            var pageLinks = new List<Guid>();

            if (tag.PermanentLinks == null)
            {
                var tempTerm = this._tagService.GetTagByName(tag.Name);

                if (tempTerm != null)
                {
                    pageLinks = tempTerm.PermanentLinks.ToList();
                }
            }
            else
            {
                pageLinks = tag.PermanentLinks.ToList();
            }

            var pages = new PageDataCollection();

            foreach (Guid pageGuid in pageLinks)
            {
                pages.Add(this._contentLoader.Get<PageData>(TagsHelper.GetContentReference(pageGuid)));
            }

            return pages;
        }

        [Obsolete("Use GetContentsByTag instead.")]
        public PageDataCollection GetPagesByTag(string tagName, PageReference rootPageReference)
        {
            return this.GetPagesByTag(this._tagService.GetTagByName(tagName));
        }

        [Obsolete("Use GetContentsByTag instead.")]
        public PageDataCollection GetPagesByTag(Tag tag, PageReference rootPageReference)
        {
            if (tag == null || tag.PermanentLinks == null)
            {
                return null;
            }

            IList<PageReference> descendantPageReferences = DataFactory.Instance.GetDescendents(rootPageReference);

            if (descendantPageReferences == null || descendantPageReferences.Count < 1)
            {
                return null;
            }

            var pages = new PageDataCollection();

            foreach (Guid pageGuid in tag.PermanentLinks)
            {
                var pageReference = TagsHelper.GetContentReference(pageGuid);

                if (descendantPageReferences.FirstOrDefault(p => p.ID == pageReference.ID) != null)
                {
                    pages.Add(this._contentLoader.Get<PageData>(pageReference));
                }
            }

            return pages;
        }

        [Obsolete("Use GetContentReferencesByTags instead.")]
        public IEnumerable<PageReference> GetPageReferencesByTags(string tagNames)
        {
            if (string.IsNullOrEmpty(tagNames))
            {
                return null;
            }

            var tags = tagNames.Split(',').Select(tagName => this._tagService.GetTagByName(tagName)).ToList();

            return GetPageReferencesByTags(tags);
        }

        [Obsolete("Use GetContentReferencesByTags instead.")]
        public IEnumerable<PageReference> GetPageReferencesByTags(IEnumerable<Tag> tags)
        {
            var matches = new Dictionary<PageReference, int>();

            foreach (Tag tag in tags)
            {
                if (tag != null && tag.PermanentLinks != null)
                {
                    foreach (Guid pageGuid in tag.PermanentLinks)
                    {
                        var pageReference = TagsHelper.GetPageReference(pageGuid);

                        if (matches.ContainsKey(pageReference))
                        {
                            matches[pageReference] += 1;
                        }
                        else
                        {
                            matches.Add(pageReference, 1);
                        }
                    }
                }
            }

            matches = matches.OrderByDescending(t => t.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            return matches.Keys;
        }

        [Obsolete("Use GetContentReferencesByTags instead.")]
        public IEnumerable<PageReference> GetPageReferencesByTags(string tagNames, PageReference rootPageReference)
        {
            IList<Tag> tags = new List<Tag>();

            foreach (string tagName in tagNames.Split(','))
            {
                tags.Add(this._tagService.GetTagByName(tagName));
            }

            return this.GetPageReferencesByTags(tags, rootPageReference);
        }

        [Obsolete("Use GetContentReferencesByTags instead.")]
        public IEnumerable<PageReference> GetPageReferencesByTags(IEnumerable<Tag> tags, PageReference rootPageReference)
        {
            if (tags == null || PageReference.IsNullOrEmpty(rootPageReference))
            {
                return null;
            }

            IList<PageReference> descendantPageReferences = DataFactory.Instance.GetDescendents(rootPageReference);

            if (descendantPageReferences == null || descendantPageReferences.Count < 1)
            {
                return null;
            }

            var matches = new Dictionary<PageReference, int>();

            foreach (Tag tag in tags)
            {
                if (tag == null || tag.PermanentLinks == null)
                {
                    continue;
                }

                foreach (Guid pageGuid in tag.PermanentLinks)
                {
                    var pageReference = TagsHelper.GetPageReference(pageGuid);

                    if (descendantPageReferences.FirstOrDefault(p => p.ID == pageReference.ID) != null)
                    {
                        if (matches.ContainsKey(pageReference))
                        {
                            matches[pageReference] += 1;
                        }
                        else
                        {
                            matches.Add(pageReference, 1);
                        }
                    }
                }
            }

            matches = matches.OrderByDescending(t => t.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            return new PageReferenceCollection(matches.Keys);
        }

        public IEnumerable<ContentData> GetContentByTag(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                return null;
            }

            return this.GetContentsByTag(this._tagService.GetTagByName(tagName));
        }

        public IEnumerable<ContentData> GetContentsByTag(Tag tag)
        {
            if (tag == null)
            {
                return null;
            }

            var contentLinks = new List<Guid>();

            if (tag.PermanentLinks == null)
            {
                var tempTerm = this._tagService.GetTagByName(tag.Name);

                if (tempTerm != null)
                {
                    contentLinks = tempTerm.PermanentLinks.ToList();
                }
            }
            else
            {
                contentLinks = tag.PermanentLinks.ToList();
            }

            return contentLinks.Select(contentGuid => this._contentLoader.Get<ContentData>(TagsHelper.GetContentReference(contentGuid))).ToList();
        }

        public IEnumerable<ContentData> GetContentsByTag(string tagName, ContentReference rootContentReference)
        {
            return this.GetContentsByTag(this._tagService.GetTagByName(tagName), rootContentReference);
        }

        public IEnumerable<ContentData> GetContentsByTag(Tag tag, ContentReference rootContentReference)
        {
            if (tag == null || tag.PermanentLinks == null)
            {
                return null;
            }

            IEnumerable<ContentReference> descendantContentReferences = this._contentLoader.GetDescendents(rootContentReference);

            if (descendantContentReferences == null || !descendantContentReferences.Any())
            {
                return null;
            }

            var items = new List<ContentData>();

            foreach (Guid contentGuid in tag.PermanentLinks)
            {
                var contentReference = TagsHelper.GetContentReference(contentGuid);

                if (descendantContentReferences.FirstOrDefault(p => p.ID == contentReference.ID) != null)
                {
                    items.Add(this._contentLoader.Get<PageData>(contentReference));
                }
            }

            return items;
        }

        public IEnumerable<ContentReference> GetContentReferencesByTags(string tagNames)
        {
            if (string.IsNullOrEmpty(tagNames))
            {
                return null;
            }

            var tags = tagNames.Split(',').Select(tagName => this._tagService.GetTagByName(tagName)).ToList();

            return GetContentReferencesByTags(tags);
        }

        public IEnumerable<ContentReference> GetContentReferencesByTags(IEnumerable<Tag> tags)
        {
            var matches = new Dictionary<ContentReference, int>();

            foreach (Tag tag in tags)
            {
                if (tag != null && tag.PermanentLinks != null)
                {
                    foreach (Guid contentGuid in tag.PermanentLinks)
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
            }

            matches = matches.OrderByDescending(t => t.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            return matches.Keys;
        }

        public IEnumerable<ContentReference> GetContentReferencesByTags(string tagNames, ContentReference rootContentReference)
        {
            IList<Tag> tags = new List<Tag>();

            foreach (string tagName in tagNames.Split(','))
            {
                tags.Add(this._tagService.GetTagByName(tagName));
            }

            return this.GetContentReferencesByTags(tags, rootContentReference);
        }

        public IEnumerable<ContentReference> GetContentReferencesByTags(IEnumerable<Tag> tags, ContentReference rootContentReference)
        {
            if (tags == null || ContentReference.IsNullOrEmpty(rootContentReference))
            {
                return null;
            }

            IEnumerable<ContentReference> descendantPageReferences = this._contentLoader.GetDescendents(rootContentReference);

            if (descendantPageReferences == null || !descendantPageReferences.Any())
            {
                return null;
            }

            var matches = new Dictionary<ContentReference, int>();

            foreach (Tag tag in tags)
            {
                if (tag == null || tag.PermanentLinks == null)
                {
                    continue;
                }

                foreach (Guid contentGuid in tag.PermanentLinks)
                {
                    var contentReference = TagsHelper.GetContentReference(contentGuid);

                    if (descendantPageReferences.FirstOrDefault(p => p.ID == contentReference.ID) != null)
                    {
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
            }

            matches = matches.OrderByDescending(t => t.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            return matches.Keys;
        }
    }
}