// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Linq;
using EPiServer.Data;
using Geta.Tags.Models;

namespace Geta.Tags.Interfaces
{
    using System;

    public interface ITagService
    {
        Tag GetTagById(Identity id);
        IEnumerable<Tag> GetTagsByContent(Guid contentGuid);
        [Obsolete("Use GetTagByNameAndGroup instead.")]
        Tag GetTagByName(string name);
        Tag GetTagByNameAndGroup(string name, string groupKey);
        IEnumerable<Tag> GetTagsByName(string name);
        IQueryable<Tag> GetAllTags();
        Identity Save(Tag tag);
        Tag Save(Guid contentGuid, string name, string groupKey);
        void Save(Guid contentGuid, IEnumerable<string> names, string groupKey);
        void Delete(string name);
        void Delete(Identity id);
    }
}