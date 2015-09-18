using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Data;
using Geta.Tags.Attributes;
using Geta.Tags.Models;

namespace Geta.Tags.Interfaces
{
    using System;

    public interface ITagService
    {
        Tag GetTagById(Identity id);
        IEnumerable<Tag> GetTagByPage(Guid pageGuid);
        Tag GetTagByName(string name);
        IQueryable<Tag> GetAllTags();
        Identity Save(Tag tag);
        Tag Save(Guid contentGuid, string name, string groupKey);
        void Save(Guid contentGuid, IEnumerable<string> names, string groupKey);
        void Delete(string name);
        void Delete(Identity id); 
    }
}