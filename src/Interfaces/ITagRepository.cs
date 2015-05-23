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
        Tag GetTagByName(string name);
        IEnumerable<Tag> GetTagsByPage(Guid pageGuid);
        IQueryable<Tag> GetAllTags();
        Identity Save(Tag tag);
        void Delete(Tag tag); 
    }
}