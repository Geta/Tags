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
using Geta.Tags.Implementations;
using Geta.Tags.Interfaces;

namespace Geta.Tags
{
    [ModuleDependency(typeof (ServiceContainerInitialization))]
    public class TagsModule : IInitializableModule
    {
        private ITagService _tagService;
        private PageTypeRepository _pageTypeRepository;

        private void OnPublishedPage(object sender, PageEventArgs e)
        {
            var page = e.Page;
            var tags = GetPageTags(page);
            _tagService.Save(page.PageGuid, tags);
        }

        private IEnumerable<string> GetPageTags(PageData page)
        {
            var pageType = _pageTypeRepository.Load(page.PageTypeID);

            return pageType.PropertyDefinitions
                .Where(TagsHelper.IsTagProperty)
                .SelectMany(x => GetPropertyTags(page, x));
        }

        private static IEnumerable<string> GetPropertyTags(PageData page, PropertyDefinition propertyDefinition)
        {
            var tagNames = page[propertyDefinition.Name] as string;
            return tagNames == null
                ? Enumerable.Empty<string>()
                : tagNames.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
        }

        public void Initialize(InitializationEngine context)
        {
            _tagService = new TagService();
            _pageTypeRepository = ServiceLocator.Current.GetInstance<PageTypeRepository>();

            DataFactory.Instance.PublishedPage += OnPublishedPage;
        }

        public void Uninitialize(InitializationEngine context)
        {
            DataFactory.Instance.PublishedPage -= OnPublishedPage;
        }

        public void Preload(string[] parameters)
        {
        }
    }
}