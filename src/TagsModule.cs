using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Geta.Tags.Attributes;
using Geta.Tags.Helpers;
using Geta.Tags.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Geta.Tags
{
    [ModuleDependency(typeof (ServiceContainerInitialization))]
    public class TagsModule : IInitializableModule
    {
        private ITagService _tagService;
        private IContentTypeRepository _contentTypeRepository;
        private IContentEvents _contentEvents;

        private void OnPublishedContent(object sender, ContentEventArgs e)
        {
            var content = e.Content;

            // If there is tags property added and removed from a content type,
            // we may want to remove its content
            CleanupOldTags(content.ContentGuid);

            var contentType = _contentTypeRepository.Load(content.ContentTypeID);
            var tagsProperties = GetTagsProperties(contentType);
            if (!tagsProperties.Any())
                return;

            // TODO: What if there are more tags properties on the same page type?
            var tagsProp = contentType.ModelType.GetProperty(tagsProperties.First().Name);
            var groupKeyAttribute = (TagsGroupKeyAttribute)tagsProp.GetCustomAttribute(typeof(TagsGroupKeyAttribute));
            var cultureSpecificAttribute = (CultureSpecificAttribute)tagsProp.GetCustomAttribute(typeof(CultureSpecificAttribute));
            string groupKey = Helpers.TagsHelper.GetGroupKeyFromAttributes(groupKeyAttribute, cultureSpecificAttribute);

            var tags = GetContentTags(content, tagsProperties);
            _tagService.Save(content.ContentGuid, tags, groupKey);
        }

        private void CleanupOldTags(Guid contentGuid)
        {
            var oldTags = _tagService.GetTagByPage(contentGuid);

            foreach (var tag in oldTags)
            {
                if (tag.PermanentLinks == null || !tag.PermanentLinks.Contains(contentGuid))
                {
                    continue;
                }

                tag.PermanentLinks.Remove(contentGuid);

                _tagService.Save(tag);
            }
        }

        private IEnumerable<PropertyDefinition> GetTagsProperties(ContentType contentType)
        {
            return contentType.PropertyDefinitions
                .Where(TagsHelper.IsTagProperty)
                .ToArray();
        }

        private IEnumerable<string> GetContentTags(IContent content, IEnumerable<PropertyDefinition> tagsProperties)
        {
            return tagsProperties.SelectMany(x => GetPropertyTags(content as ContentData, x));
        }

        private static IEnumerable<string> GetPropertyTags(ContentData content, PropertyDefinition propertyDefinition)
        {
            var tagNames = content[propertyDefinition.Name] as string;
            return tagNames == null
                ? Enumerable.Empty<string>()
                : tagNames.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
        }

        public void Initialize(InitializationEngine context)
        {
            this._tagService = ServiceLocator.Current.GetInstance<ITagService>();
            this._contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            this._contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();

            this._contentEvents.PublishedContent += OnPublishedContent;
        }

        public void Uninitialize(InitializationEngine context)
        {
            this._contentEvents.PublishedContent -= OnPublishedContent;
        }
    }
}