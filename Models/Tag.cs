using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace Geta.Tags.Models
{
    using System;
    using System.Collections.Generic;

    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
    public class Tag
    {
        public Identity Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Lowercase, words combined by - between them
        /// </summary>
        public string Slug { get; set; }

        public string Description { get; set; }

        // number of objects, pages that use this term
        public int Count { get; set; }

        public IList<Guid> PermanentLinks { get; set; }

        public override int GetHashCode()
        {
            if (this.Name == null)
            {
                return base.GetHashCode();
            }

            return this.Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            Tag tag = (Tag)obj;

            if (!Equals(this.Name, tag.Name))
            {
                return false;
            }

            return true;
        } 
    }
}