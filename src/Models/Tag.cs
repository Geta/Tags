// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace Geta.Tags.Models
{
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
    public class Tag
    {
        public Identity Id { get; set; }

        public string Name { get; set; }

        public string GroupKey { get; set; }

        /// <summary>
        ///     Lowercase, words combined by - between them
        /// </summary>
        public string Slug { get; set; }

        public string Description { get; set; }

        // number of objects, pages that use this term
        public int Count => PermanentLinks?.Count ?? 0;

        public IList<Guid> PermanentLinks { get; set; }

        public bool checkedEditContentTags { get; set; }

        public override int GetHashCode()
        {
            return Name == null ? base.GetHashCode() : Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            var tag = (Tag) obj;

            return Equals(Name, tag.Name);
        }
    }
}