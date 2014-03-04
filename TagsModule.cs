using System;
using System.Collections.Generic;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using Geta.Tags.Implementations;
using Geta.Tags.Interfaces;

namespace Geta.Tags
{
    [ModuleDependency(typeof (EPiServer.Web.InitializationModule))]
    public class TagsModule : IInitializableModule
    {
        public ITagService TagService { get; set; }

        public TagsModule()
        {
            TagService = new TagService();
        }

        private void OnPublishedPage(object sender, PageEventArgs e)
        {
            var page = e.Page;
            var pageType = PageType.Load(page.PageTypeID);

            foreach (var pageDefinition in pageType.Definitions)
            {
                if (pageDefinition == null || pageDefinition.Type == null)
                {
                    continue;
                }

                if (IsNotTagProperty(pageDefinition)) continue;

                var tagNames = page[pageDefinition.Name] as string;
                if (tagNames == null) continue;

                var tags = tagNames.Split(',');

                Store(page, tags);
            }
        }

        private void Store(PageData page, IEnumerable<string> tags)
        {
            foreach (var tag in tags)
            {
                TagService.Save(page.PageGuid, tag);
            }
        }

        private static bool IsNotTagProperty(PageDefinition pageDefinition)
        {
            return !pageDefinition.Type.DefinitionType.Name.Equals("PropertyTags", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Initialize(InitializationEngine context)
        {
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