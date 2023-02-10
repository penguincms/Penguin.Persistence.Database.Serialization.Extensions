using Penguin.Persistence.Abstractions.Attributes.Rendering;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Abstractions.Wrappers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Penguin.Persistence.Database.Serialization.Extensions.Meta
{
    public class DbMetaProperty : IMetaProperty
    {
        public List<IMetaAttribute> Attributes { get; set; } = new List<IMetaAttribute>()
        {
                new MetaAttributeHolder(new DisplayAttribute()
                {
                    GroupName = "grid"
                }, false)
        };

        IEnumerable<IMetaAttribute> IHasAttributes.Attributes => Attributes;
        public DbMetaType DeclaringType { get; set; }
        IMetaType IMetaProperty.DeclaringType => DeclaringType;
        public string Name { get; set; }
        public IMetaType Type { get; set; }

        public IMetaType TypeOf()
        {
            return Type;
        }

        internal static DbMetaProperty FromValueProperty(PropertyInfo pi)
        {
            throw new NotImplementedException();
        }
    }
}