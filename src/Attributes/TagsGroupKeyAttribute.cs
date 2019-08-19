// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace Geta.Tags.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TagsGroupKeyAttribute : Attribute
    {
        public TagsGroupKeyAttribute(string key)
        {
            Key = key;
        }

        public virtual string Key { get; }
    }
}
