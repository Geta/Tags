using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Geta.Tags.Helpers;
using Geta.Tags.Interfaces;

namespace Geta.Tags
{
    [ModuleDependency(typeof (ServiceContainerInitialization))]
    public class TagsModule : IInitializableModule
    {
        private ITagService _tagService;
        private ContentTypeRepository _contentTypeRepository;
        private IContentEvents _contentEvents;

        private void OnPublishedContent(object sender, ContentEventArgs e)
        {
            var content = e.Content;
            var tags = GetContentTags(content);
            _tagService.Save(content.ContentGuid, tags);
        }

        private IEnumerable<string> GetContentTags(IContent content)
        {
            var pageType = _contentTypeRepository.Load(content.ContentTypeID);

            return pageType.PropertyDefinitions
                .Where(TagsHelper.IsTagProperty)
                .SelectMany(x => GetPropertyTags(content as ContentData, x));
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
            this._contentTypeRepository = ServiceLocator.Current.GetInstance<ContentTypeRepository>();
            this._contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();

            this._contentEvents.PublishedContent += OnPublishedContent;
        }

        public void Uninitialize(InitializationEngine context)
        {
            this._contentEvents.PublishedContent -= OnPublishedContent;
        }

        public void Preload(string[] parameters)
        {
        }
    }
}