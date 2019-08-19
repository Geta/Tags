// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;
using Geta.Tags.Interfaces;
using Geta.Tags.Models;
using Geta.Tags.Helpers;

namespace Geta.Tags
{
    [ScheduledPlugIn(DisplayName = "Geta Tags maintenance", DefaultEnabled = true)]
    public class TagsScheduledJob : ScheduledJobBase
    {
        private bool _stop;

        private readonly ITagService _tagService;
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly IContentLoader _contentLoader;

        public TagsScheduledJob()
        {
            IsStoppable = true;
            _contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            _tagService = ServiceLocator.Current.GetInstance<ITagService>();
            _contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
        }

        public override string Execute()
        {
            var tags = _tagService.GetAllTags().ToList();
            var contentGuids = GetTaggedContentGuids(tags);

            foreach (var contentGuid in contentGuids)
            {
                if (_stop)
                {
                    return "Geta Tags maintenance was stopped";
                }

                IContent content = null;

                try
                {
                    var contentReference = TagsHelper.GetContentReference(contentGuid);

                    if (!ContentReference.IsNullOrEmpty(contentReference))
                    {
                        content = _contentLoader.Get<IContent>(contentReference);
                    }
                }
                catch (ContentNotFoundException) {}

                if (content == null || (content is PageData data && data.IsDeleted))
                {
                    RemoveFromAllTags(contentGuid, tags);
                    continue;
                }

                CheckContentProperties(content, tags);
            }

            return "Geta Tags maintenance completed successfully";
        }

        private void CheckContentProperties(IContent content, IList<Tag> tags)
        {
            var contentType = _contentTypeRepository.Load(content.ContentTypeID);

            foreach (var propertyDefinition in contentType.PropertyDefinitions)
            {
                if (!TagsHelper.IsTagProperty(propertyDefinition))
                {
                    continue;
                }

                var tagNames = GetTagNames(content, propertyDefinition);

                var allTags = tags;

                if (tagNames == null)
                {
                    RemoveFromAllTags(content.ContentGuid, allTags);
                    continue;
                }

                var addedTags = ParseTags(tagNames);

                // make sure the tags it has added has the ContentReference
                ValidateTags(allTags, content.ContentGuid, addedTags);

                // make sure there's no ContentReference to this ContentReference in the rest of the tags
                RemoveFromAllTags(content.ContentGuid, allTags);
            }
        }

        private string GetTagNames(IContent content, PropertyDefinition propertyDefinition)
        {
            if (content is ILocalizable)
            {
                return GetAllLanguageTagNames(content, propertyDefinition);
            }
            return ((ContentData)content)[propertyDefinition.Name] as string;
        }

        private string GetAllLanguageTagNames(IContent localizableContent, PropertyDefinition tagPropertyDefinition)
        {
            var localizable = (ILocalizable)localizableContent;
            var tags = localizable
                .ExistingLanguages
                .Select(language => _contentLoader.Get<IContent>(localizableContent.ContentGuid, language))
                .Select(x => ((ContentData)x)[tagPropertyDefinition.Name] as string);
            return string.Join(",", tags);
        }

        private static IEnumerable<Guid> GetTaggedContentGuids(IEnumerable<Tag> tags)
        {
            return tags.Where(x => x?.PermanentLinks != null)
                .SelectMany(x => x.PermanentLinks)
                .ToList();
        }

        private IEnumerable<Tag> ParseTags(string tagNames)
        {
            return tagNames.Split(',')
                .SelectMany(_tagService.GetTagsByName)
                .Where(tag => tag != null)
                .ToList();
        }

        private void ValidateTags(ICollection<Tag> allTags, Guid contentGuid, IEnumerable<Tag> addedTags)
        {
            foreach (var addedTag in addedTags)
            {
                allTags.Remove(addedTag);

                if (addedTag.PermanentLinks.Contains(contentGuid)) continue;

                addedTag.PermanentLinks.Add(contentGuid);

                _tagService.Save(addedTag);
            }
        }

        private void RemoveFromAllTags(Guid guid, IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                if (tag.PermanentLinks == null || !tag.PermanentLinks.Contains(guid)) continue;

                tag.PermanentLinks.Remove(guid);

                _tagService.Save(tag);
            }
        }

        public override void Stop()
        {
            _stop = true;
        }
    }
}