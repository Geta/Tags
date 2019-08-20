// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Geta.Tags.Attributes;
using Geta.Tags.Helpers;
using Geta.Tags.Interfaces;

namespace Geta.Tags
{
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class TagsModule : IInitializableModule
    {
        private ITagService _tagService;
        private IContentTypeRepository _contentTypeRepository;
        private IContentEvents _contentEvents;

        private void OnPublishedContent(object sender, ContentEventArgs e)
        {
            var content = e.Content;

            CleanupOldTags(content);

            var contentType = _contentTypeRepository.Load(content.ContentTypeID);

            var tagProperties = contentType.PropertyDefinitions.Where(p => p.TemplateHint == "Tags").ToArray();

            if (!tagProperties.Any())
            {
                return;
            }

            foreach (var tagProperty in tagProperties)
            {
                var tagPropertyInfo = contentType.ModelType.GetProperty(tagProperty.Name);
                var tags = GetPropertyTags(content as ContentData, tagProperty);

                if (tagPropertyInfo == null)
                {
                    return;
                }

                var groupKeyAttribute =
                    tagPropertyInfo.GetCustomAttribute(typeof(TagsGroupKeyAttribute)) as TagsGroupKeyAttribute;
                var cultureSpecificAttribute
                    = tagPropertyInfo.GetCustomAttribute(typeof(CultureSpecificAttribute)) as CultureSpecificAttribute;

                var groupKey = TagsHelper.GetGroupKeyFromAttributes(groupKeyAttribute, cultureSpecificAttribute, content);

                _tagService.Save(content.ContentGuid, tags, groupKey);
            }
        }

        private void CleanupOldTags(IContent content)
        {
            var contentGuid = content.ContentGuid;
            var oldTags = _tagService.GetTagsByContent(contentGuid);
            var language = content.LanguageBranch();

            foreach (var tag in oldTags)
            {
                if (tag.PermanentLinks == null || !tag.PermanentLinks.Contains(contentGuid) || tag.GroupKey != language)
                {
                    continue;
                }

                tag.PermanentLinks.Remove(contentGuid);

                _tagService.Save(tag);
            }
        }

        private static IEnumerable<string> GetPropertyTags(ContentData content, PropertyDefinition propertyDefinition)
        {
            var tagNames = content[propertyDefinition.Name] as string;
            return tagNames?.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries) ?? Enumerable.Empty<string>();
        }

        public void Initialize(InitializationEngine context)
        {
            _tagService = ServiceLocator.Current.GetInstance<ITagService>();
            _contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            _contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();

            _contentEvents.PublishedContent += OnPublishedContent;
        }

        public void Uninitialize(InitializationEngine context)
        {
            _contentEvents.PublishedContent -= OnPublishedContent;
        }
    }
}