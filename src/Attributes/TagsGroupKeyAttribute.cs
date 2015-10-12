using System;

namespace Geta.Tags.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TagsGroupKeyAttribute : Attribute
    {
        private string key;
        public TagsGroupKeyAttribute(string key)
        {
            this.key = key;
        }

        public virtual string Key
        {
            get { return key; }
        }
    }
}