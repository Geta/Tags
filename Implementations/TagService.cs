using System.Collections.Generic;
using System.Linq;
using EPiServer.Data;
using Geta.Tags.Interfaces;
using Geta.Tags.Models;

namespace Geta.Tags.Implementations
{
    using System;

    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService() : this(new TagRepository())
        {
        }

        public TagService(ITagRepository tagRepository)
        {
            this._tagRepository = tagRepository;
        }

        public Tag GetTagById(Identity id)
        {
            return this._tagRepository.GetTagById(id);
        }

        public IEnumerable<Tag> GetTagByPage(Guid pageGuid)
        {
            return this._tagRepository.GetTagsByPage(pageGuid);
        }

        public Tag GetTagByName(string name)
        {
            return this._tagRepository.GetTagByName(name);
        }

        public IQueryable<Tag> GetAllTags()
        {
            return this._tagRepository.GetAllTags();
        }

        public Identity Save(Tag tag)
        {
            return this._tagRepository.Save(tag);
        }

        public Tag Save(Guid pageGuid, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            Tag tag = this.GetTagByName(name);

            if (tag == null)
            {
                tag = new Tag
                {
                    Count = 1,
                    Name = name,
                    PermanentLinks = new List<Guid>() { pageGuid }
                };
            }
            else
            {
                if (tag.PermanentLinks == null)
                {
                    tag.PermanentLinks = new List<Guid>() { pageGuid };
                    tag.Count = tag.Count + 1;
                }
                else
                {
                    if (!tag.PermanentLinks.Contains(pageGuid))
                    {
                        tag.PermanentLinks.Add(pageGuid);

                        tag.Count = tag.Count + 1;
                    }
                }
            }

            this.Save(tag);

            return tag;
        }

        public void Save(Guid pageGuid, IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                Save(pageGuid, name);
            }
        }

        public void Delete(string name)
        {
            Tag tag = this.GetTagByName(name);

            if (tag == null)
            {
                return;
            }

            this._tagRepository.Delete(tag);
        }

        public void Delete(Identity id)
        {
            Tag tag = this._tagRepository.GetTagById(id);

            if (tag == null)
            {
                return;
            }

            this._tagRepository.Delete(tag);
        }
    }
}