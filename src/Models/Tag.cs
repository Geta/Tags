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
        public int Count
        {
            get { return PermanentLinks == null ? 0 : PermanentLinks.Count; }
        }

        public IList<Guid> PermanentLinks { get; set; }

        public override int GetHashCode()
        {
            if (Name == null)
            {
                return base.GetHashCode();
            }

            return Name.GetHashCode();
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

            if (!Equals(Name, tag.Name))
            {
                return false;
            }

            return true;
        }
    }
}