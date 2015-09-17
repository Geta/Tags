using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Geta.Tags.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TagsGroupKey : Attribute
    {
        private string key;
        public TagsGroupKey(string key)
        {
            this.key = key;
        }

        public virtual string Key
        {
            get { return key; }
        }
    }
}