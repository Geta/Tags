using System.Collections.Generic;
using System.Linq;
using EPiServer.Data;
using Geta.Tags.Models;

namespace Geta.Tags.Interfaces
{
    using System;

    public interface ITagRepository
    {
        Tag GetTagById(Identity id);
        [Obsolete("Use GetTagByNameAndGroup instead.")]
        Tag GetTagByName(string name);
        Tag GetTagByNameAndGroup(string name, string groupKey);
        IEnumerable<Tag> GetTagsByName(string name);
        IEnumerable<Tag> GetTagsByContent(Guid contentGuid);
        IQueryable<Tag> GetAllTags();
        Identity Save(Tag tag);
        void Delete(Tag tag);
    }
}