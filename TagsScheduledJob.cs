using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.BaseLibrary.Scheduling;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.PlugIn;
using Geta.Tags.Implementations;
using Geta.Tags.Interfaces;
using Geta.Tags.Models;
using Geta.Tags.Helpers;

namespace Geta.Tags
{
    [ScheduledPlugIn(DisplayName = "Geta Tags maintenance", DefaultEnabled = true)]
    public class TagsScheduledJob : JobBase
    {
        private bool _stop;

        private readonly ITagService _tagService;

        public TagsScheduledJob() : this(new TagService())
        {
            IsStoppable = true;
        }

        public TagsScheduledJob(ITagService tagService)
        {
            _tagService = tagService;
        }

        public override string Execute()
        {
            var tags = _tagService.GetAllTags().ToList();
            var pageGuids = GetTaggedPageGuids(tags);

            foreach (var pageGuid in pageGuids)
            {
                if (_stop)
                {
                    return "Geta Tags maintenance was stopped";
                }

                PageData page = null;

                try
                {
                    page = DataFactory.Instance.GetPage(TagsHelper.GetPageReference(pageGuid));
                }
                catch (PageNotFoundException) {}

                if (page == null || page.IsDeleted)
                {
                    RemoveFromAllTags(pageGuid, tags);
                    continue;
                }

                CheckPageProperties(page, tags);
            }

            UpdateTagCount();

            return "Geta Tags maintenance completed successfully";
        }

        private void CheckPageProperties(PageData page, List<Tag> tags)
        {
            // Get property names from page type
            var pageType = PageType.Load(page.PageTypeID);

            foreach (var pageDefinition in pageType.Definitions)
            {
                if (pageDefinition == null || pageDefinition.Type == null)
                {
                    continue;
                }

                if (IsNotTagProperty(pageDefinition)) continue;

                var tagNames = page[pageDefinition.Name] as string;

                IList<Tag> allTags = tags;

                if (tagNames == null)
                {
                    RemoveFromAllTags(page.PageGuid, allTags);
                    continue;
                }

                var addedTags = ParseTags(tagNames);

                // make sure the tags it has added has the pagereference
                ValidateTags(allTags, page.PageGuid, addedTags);

                // make sure there's no pagereference to this pagereference in the rest of the tags
                RemoveFromAllTags(page.PageGuid, allTags);
            }
        }

        private static IEnumerable<Guid> GetTaggedPageGuids(IEnumerable<Tag> tags)
        {
            return tags.Where(x => x != null && x.PermanentLinks != null)
                .SelectMany(x => x.PermanentLinks);
        }

        private void UpdateTagCount()
        {
            var tags = _tagService.GetAllTags();

            foreach (var tag in tags)
            {
                tag.Count = tag.PermanentLinks == null 
                    ? 0 
                    : tag.PermanentLinks.Count;

                _tagService.Save(tag);
            }
        }

        private IEnumerable<Tag> ParseTags(string tagNames)
        {
            return tagNames.Split(',')
                .Select(_tagService.GetTagByName)
                .Where(tag => tag != null)
                .ToList();
        }

        private void ValidateTags(ICollection<Tag> allTags, Guid pageGuid, IEnumerable<Tag> addedTags)
        {
            foreach (var addedTag in addedTags)
            {
                allTags.Remove(addedTag);

                if (addedTag.PermanentLinks.Contains(pageGuid)) continue;

                addedTag.PermanentLinks.Add(pageGuid);

                _tagService.Save(addedTag);
            }
        }

        private static bool IsNotTagProperty(PageDefinition pageDefinition)
        {
            return !pageDefinition.Type.DefinitionType.Name.Equals("PropertyTags", StringComparison.InvariantCultureIgnoreCase);
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