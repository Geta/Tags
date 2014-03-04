using System;
using System.Collections.Generic;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
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
            var pageType = _pageTypeRepository.Load(page.PageTypeID);

            foreach (var propertyDefinition in pageType.PropertyDefinitions)
            {
                if (propertyDefinition == null || propertyDefinition.Type == null)
                {
                    continue;
                }

                if (IsNotTagProperty(propertyDefinition)) continue;

                var tagNames = page[propertyDefinition.Name] as string;
                if (tagNames == null) continue;

                var tags = tagNames.Split(',');

                Store(page, tags);
            }
        }

        private void Store(PageData page, IEnumerable<string> tags)
        {
            foreach (var tag in tags)
            {
                _tagService.Save(page.PageGuid, tag);
            }
        }

        private static bool IsNotTagProperty(PropertyDefinition pageDefinition)
        {
            return
                !pageDefinition.Type.DefinitionType.Name.Equals("PropertyTags",
                    StringComparison.InvariantCultureIgnoreCase);
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