// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;
using Geta.Tags.Interfaces;
using Geta.Tags.Models;

namespace Geta.Tags.Implementations
{
    [ServiceConfiguration(typeof(ITagRepository))]
    public class TagRepository : ITagRepository
    {
        private static DynamicDataStore TagStore => typeof(Tag).GetOrCreateStore();

        public Tag GetTagById(Identity id)
        {
            return TagStore.Load<Tag>(id);
        }

        public Tag GetTagByName(string name)
        {
            return TagStore.Find<Tag>("Name", name).FirstOrDefault();
        }

        public Tag GetTagByNameAndGroup(string name, string groupKey)
        {
            return TagStore
                .Find<Tag>(new Dictionary<string, object>
                {
                    { "Name", name }, { "GroupKey", groupKey }
                })
                .FirstOrDefault();
        }

        public IEnumerable<Tag> GetTagsByName(string name)
        {
            return TagStore.Find<Tag>("Name", name);
        }

        public IEnumerable<Tag> GetTagsByContent(Guid contentGuid)
        {
            return GetAllTags().Where(t => t.PermanentLinks.Contains(contentGuid));
        }

        public IQueryable<Tag> GetAllTags()
        {
            return TagStore.Items<Tag>();
        }

        public Identity Save(Tag tag)
        {
            if (string.IsNullOrEmpty(tag?.Name))
            {
                return null;
            }

            var existingTag = GetTagByNameAndGroup(tag.Name, tag.GroupKey);
            return existingTag != null
                ? TagStore.Save(tag, existingTag.GetIdentity())
                : TagStore.Save(tag);
        }

        public void Delete(Tag tag)
        {
            TagStore.Delete(tag);
        }
    }
}