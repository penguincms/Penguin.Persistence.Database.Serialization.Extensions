using Penguin.Persistence.Abstractions.Attributes.Rendering;
using Penguin.Reflection.Abstractions;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Abstractions.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Penguin.Persistence.Database.Serialization.Extensions.Meta
{
    public class DbMetaObject : IMetaObject
    {
        public List<DbMetaObject> CollectionItems { get; set; }

        IReadOnlyList<IMetaObject> IMetaObject.CollectionItems => CollectionItems;

        public bool Null { get; }

        public DbMetaObject Parent { get; set; }

        IMetaObject IMetaObject.Parent
        {
            get => Parent;
            set => Parent = (DbMetaObject)value;
        }

        public List<DbMetaObject> Properties { get; set; } = new List<DbMetaObject>();

        IReadOnlyList<IMetaObject> IMetaObject.Properties => Properties;

        public DbMetaProperty Property { get; set; } = new DbMetaProperty();

        IMetaProperty IMetaObject.Property => Property;

        public DbMetaObject Template { get; set; }

        IMetaObject IMetaObject.Template => Template;

        public IMetaType Type { get; set; }

        public string Value { get; set; }

        public DbMetaObject this[string PropertyName] => this.Properties.FirstOrDefault(p => p.Property.Name == PropertyName);

        IMetaObject IMetaObject.this[IMetaProperty metaProperty] => this[metaProperty.Name];

        IMetaObject IMetaObject.this[string PropertyName] => this[PropertyName];

        public static DbMetaObject FromSimpleAttribute<T>(T source) where T : Attribute
        {
            DbMetaObject o = new DbMetaObject();

            o.Properties = (source?.GetType() ?? typeof(T)).GetProperties().Select(p => DbMetaObject.FromSimpleProperty(p, o, source)).ToList();

            return o;
        }

        public static DbMetaObject FromSimpleProperty(PropertyInfo pi, DbMetaObject DbmParent, object oParent)
        {
            return new DbMetaObject()
            {
                Property = DbMetaProperty.FromValueProperty(pi),
                Parent = DbmParent,
                Value = pi.GetValue(oParent).ToString(),
                Type = DbMetaType.FromValueType(pi.PropertyType)
            };
        }

        public CoreType GetCoreType() => CoreType.Reference;

        public bool HasProperty(string propertyName) => this.Properties.Any(p => p.Property.Name == propertyName);

        public bool IsRecursive() => false;
        public IMetaType TypeOf() => this.Type;
    }
}