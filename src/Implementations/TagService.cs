using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.DataAnnotations;
using EPiServer.ServiceLocation;
using Geta.Tags.Attributes;
using Geta.Tags.Interfaces;
using Geta.Tags.Models;

namespace Geta.Tags.Implementations
{
    [ServiceConfiguration(typeof(ITagService))]
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public Tag GetTagById(Identity id)
        {
            return _tagRepository.GetTagById(id);
        }

        public IEnumerable<Tag> GetTagByPage(Guid pageGuid)
        {
            return _tagRepository.GetTagsByPage(pageGuid);
        }

        public Tag GetTagByName(string name)
        {
            return _tagRepository.GetTagByName(name);
        }

        public IQueryable<Tag> GetAllTags()
        {
            return _tagRepository.GetAllTags();
        }

        public Identity Save(Tag tag)
        {
            return _tagRepository.Save(tag);
        }

        public Tag Save(Guid contentGuid, string name, string groupKey="")
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            var tag = GetTagByName(name)
                      ?? new Tag
                      {
                          Name = name
                      };

            tag.GroupKey = groupKey;

            if (tag.PermanentLinks == null)
            {
                tag.PermanentLinks = new List<Guid> { contentGuid };
            }
            else
            {
                if (!tag.PermanentLinks.Contains(contentGuid))
                {
                    tag.PermanentLinks.Add(contentGuid);
                }
            }

            Save(tag);

            return tag;
        }

        public void Save(IContent content, IEnumerable<string> names)
        {
            TagsGroupKey groupKeyAttribute =
                   (TagsGroupKey)Attribute.GetCustomAttribute(typeof(IContent), typeof(TagsGroupKey));
            CultureSpecificAttribute cultureSpecificAttribute =
                (CultureSpecificAttribute)Attribute.GetCustomAttribute(typeof(IContent), typeof(CultureSpecificAttribute));
            string groupKey = Helpers.TagsHelper.GetGroupKeyFromAttributes(groupKeyAttribute, cultureSpecificAttribute);
            foreach (var name in names)
            {
                Save(content.ContentGuid, name, groupKey);
            }
        }

        public void Delete(string name)
        {
            Tag tag = GetTagByName(name);

            if (tag == null)
            {
                return;
            }

            _tagRepository.Delete(tag);
        }

        public void Delete(Identity id)
        {
            Tag tag = _tagRepository.GetTagById(id);

            if (tag == null)
            {
                return;
            }

            _tagRepository.Delete(tag);
        }
    }
}