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
            this._tagService = tagService;
        }

        public override string Execute()
        {
            List<Tag> tags = this._tagService.GetAllTags().ToList();

            var pageGuids = new List<Guid>();
            
            // Find all pages that have a tag property on them
            foreach (Tag tag in tags)
            {
                if (tag != null && tag.PermanentLinks != null)
                {
                    pageGuids.AddRange(tag.PermanentLinks);
                }
            }

            foreach (Guid pageGuid in pageGuids)
            {
                if (this._stop)
                {
                    return "Geta Tags maintenance was stopped";
                }

                PageData page = null;

                try
                {
                    page = DataFactory.Instance.GetPage(TagsHelper.GetPageReference(pageGuid));
                }
                catch (Exception exception)
                {
                    this.RemoveFromAllTags(pageGuid, tags);
                }

                if (page == null)
                {
                    continue;
                }

                if (page.IsDeleted)
                {
                    // remove pageReference from all tags
                    this.RemoveFromAllTags(pageGuid, tags);
                }

                // Get property names from page type
                PageType pageType = PageType.Load(page.PageTypeID);

                foreach (PageDefinition pageDefinition in pageType.Definitions)
                {
                    if (pageDefinition == null || pageDefinition.Type == null)
                    {
                        continue;
                    }

                    if (this.IsTagProperty(pageDefinition))
                    {
                        string tagNames = page[pageDefinition.Name] as string;

                        IList<Tag> allTags = tags;

                        if (tagNames == null)
                        {
                            this.RemoveFromAllTags(pageGuid, allTags);

                            continue;
                        }

                        IEnumerable<Tag> addedTags = this.ParseTags(tagNames);

                        // make sure the tags it has added has the pagereference
                        this.ValidateTags(allTags, pageGuid, addedTags);

                        // make sure there's no pagereference to this pagereference in the rest of the tags
                        this.RemoveFromAllTags(pageGuid, allTags);
                    }
                }
            }

            this.UpdateTagCount();

            return "Geta Tags maintenance completed successfully";
        }

        private void UpdateTagCount()
        {
            var tags = this._tagService.GetAllTags();

            foreach (Tag tag in tags)
            {
                if (tag.PermanentLinks == null)
                {
                    tag.Count = 0;
                }
                else
                {
                    tag.Count = tag.PermanentLinks.Count;
                }

                this._tagService.Save(tag);
            }
        }

        private IEnumerable<Tag> ParseTags(string tagNames)
        {
            IList<Tag> addedTags = new List<Tag>();

            foreach (string tagName in tagNames.Split(','))
            {
                Tag tag = this._tagService.GetTagByName(tagName);

                if (tag == null)
                {
                    continue;
                }

                addedTags.Add(tag);
            }

            return addedTags;
        }

        private void ValidateTags(IList<Tag> allTags, Guid pageGuid, IEnumerable<Tag> addedTags)
        {
            foreach (Tag addedTag in addedTags)
            {
                if (allTags.Remove(addedTag))
                {
                    
                }

                if (!addedTag.PermanentLinks.Contains(pageGuid))
                {
                    addedTag.PermanentLinks.Add(pageGuid);

                    this._tagService.Save(addedTag);
                }
            }
        }

        private bool IsTagProperty(PageDefinition pageDefinition)
        {
            return pageDefinition.Type.DefinitionType.Name.Equals("PropertyTags", StringComparison.InvariantCultureIgnoreCase);
        }

        private void RemoveFromAllTags(Guid guid, IEnumerable<Tag> tags)
        {
            foreach (Tag tag in tags)
            {
                if (tag.PermanentLinks != null && tag.PermanentLinks.Contains(guid))
                {
                    tag.PermanentLinks.Remove(guid);

                    this._tagService.Save(tag);
                }
            }
        }

        public override void Stop()
        {
            _stop = true;
        }
    }
}