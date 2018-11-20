﻿using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Data;
using EPiServer.ServiceLocation;
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

        public Tag Save(Guid contentGuid, string name, string groupKey = "")
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

        public void Save(Guid contentGuid, IEnumerable<string> names, string groupKey)
        {
            foreach (var name in names)
            {
                Save(contentGuid, name, groupKey);
            }
        }

        public void Delete(string name)
        {
            var tag = GetTagByName(name);

            if (tag == null)
            {
                return;
            }

            _tagRepository.Delete(tag);
        }

        public void Delete(Identity id)
        {
            var tag = _tagRepository.GetTagById(id);

            if (tag == null)
            {
                return;
            }

            _tagRepository.Delete(tag);
        }
    }
}