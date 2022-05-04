using Penguin.Reflection.Abstractions;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Penguin.Persistence.Database.Serialization.Extensions.Meta
{
    public class DbMetaObject : IMetaObject
    {
        public DbMetaObject this[string PropertyName] => this.Properties.FirstOrDefault(p => p.Property.Name == PropertyName);
        IMetaObject IMetaObject.this[IMetaProperty metaProperty] => this[metaProperty.Name];
        IMetaObject IMetaObject.this[string PropertyName] => this[PropertyName];
        public List<DbMetaObject> CollectionItems { get; set; }

        IReadOnlyList<IMetaObject> IMetaObject.CollectionItems => this.CollectionItems;

        public bool Null { get; }

        public DbMetaObject Parent { get; set; }

        IMetaObject IMetaObject.Parent
        {
            get => this.Parent;
            set => this.Parent = (DbMetaObject)value;
        }

        public List<DbMetaObject> Properties { get; set; } = new List<DbMetaObject>();

        IReadOnlyList<IMetaObject> IMetaObject.Properties => this.Properties;

        public DbMetaProperty Property { get; set; } = new DbMetaProperty();

        IMetaProperty IMetaObject.Property => this.Property;

        public DbMetaObject Template { get; set; }

        IMetaObject IMetaObject.Template => this.Template;

        public IMetaType Type { get; set; }

        public string Value { get; set; }

        public static DbMetaObject FromSimpleAttribute<T>(T source) where T : Attribute
        {
            DbMetaObject o = new DbMetaObject();

            o.Properties = (source?.GetType() ?? typeof(T)).GetProperties().Select(p => DbMetaObject.FromSimpleProperty(p, o, source)).ToList();

            return o;
        }

        public static DbMetaObject FromSimpleProperty(PropertyInfo pi, DbMetaObject DbmParent, object oParent)
        {
            if (pi is null)
            {
                throw new ArgumentNullException(nameof(pi));
            }

            return new DbMetaObject()
            {
                Property = DbMetaProperty.FromValueProperty(pi),
                Parent = DbmParent,
                Value = pi.GetValue(oParent).ToString(),
                Type = DbMetaType.FromValueType(pi.PropertyType)
            };
        }

        public CoreType GetCoreType()
        {
            return CoreType.Reference;
        }

        public bool HasProperty(string propertyName)
        {
            return this.Properties.Any(p => p.Property.Name == propertyName);
        }

        public bool IsRecursive()
        {
            return false;
        }

        public IMetaType TypeOf()
        {
            return this.Type;
        }
    }
}