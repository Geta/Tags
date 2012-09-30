using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using Geta.Tags.Interfaces;
using Geta.Tags.Models;

namespace Geta.Tags.Implementations
{
    using System;

    public class TagRepository : ITagRepository
    {
        private static DynamicDataStore TagStore
        {
            get
            {
                return typeof(Tag).GetStore();
            }
        }

        public Tag GetTagById(Identity id)
        {
            return TagStore.Load<Tag>(id);
        }

        public Tag GetTagByName(string name)
        {
            return TagStore.Find<Tag>("Name", name).FirstOrDefault();
        }

        public IEnumerable<Tag> GetTagsByPage(Guid pageGuid)
        {
            return this.GetAllTags().Where(t => t.PermanentLinks.Contains(pageGuid));
        }

        public IQueryable<Tag> GetAllTags()
        {
            return TagStore.Items<Tag>();
        }

        public Identity Save(Tag tag)
        {
            if (tag == null || string.IsNullOrEmpty(tag.Name))
            {
                return null;
            }

            Tag existingTag = this.GetTagByName(tag.Name);
            if (existingTag != null)
            {
                return TagStore.Save(tag, existingTag.GetIdentity());
            }

            return TagStore.Save(tag);
        }

        public void Delete(Tag tag)
        {
            TagStore.Delete(tag);
        }
    }
}