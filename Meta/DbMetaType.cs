using Penguin.Reflection.Abstractions;
using Penguin.Reflection.Extensions;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Penguin.Persistence.Database.Serialization.Extensions.Meta
{
    public class DbMetaType : IMetaType
    {
        public string AssemblyQualifiedName { get; set; }

        public IEnumerable<IMetaAttribute> Attributes { get; }

        public CoreType CoreType { get; set; }

        public string Default { get; set; }

        public string FullName { get; set; }

        public bool IsArray { get; set; }

        public bool IsEnum { get; set; }

        public bool IsNullable { get; set; }

        public bool IsNumeric { get; set; }

        public string Name { get; set; }

        public string Namespace { get; set; }

        public IReadOnlyList<IMetaType> Parameters { get; }

        public IReadOnlyList<IMetaProperty> Properties { get; }

        public IReadOnlyList<IEnumValue> Values { get; private set; }

        IMetaType IMetaType.BaseType => BaseType;

        IMetaType IMetaType.CollectionType => CollectionType;

        public DbMetaType()
        {
        }

        //    foreach(PropertyInfo pi in t.GetProperties())
        //    {
        //        props.Add(new DbMetaProperty))
        //    }
        //}
        public DbMetaType(string Name, IEnumerable<DbMetaObject> properties)
        {
            this.Name = AssemblyQualifiedName = Name;
            Namespace = "Dynamic";

            Properties = properties.Select(p => p.Property).ToList();
        }

        public static DbMetaType FromValueType(Type t)
        {
            return t is null
                ? throw new ArgumentNullException(nameof(t))
                : new DbMetaType()
                {
                    AssemblyQualifiedName = t.AssemblyQualifiedName,
                    FullName = t.FullName,
                    IsArray = t.IsArray,
                    Default = t.GetDefaultValue().ToString(),
                    IsEnum = t.IsEnum,
                    IsNumeric = t.IsNumericType(),
                    Name = t.Name,
                    Namespace = t.Namespace,
                    Values = MetaType.GetEnumValues(t)
                };
        }

        //public static DbMetaType FromComplexType(Type t)
        //{
        //    if (t is null)
        //    {
        //        throw new ArgumentNullException(nameof(t));
        //    }

        //    DbMetaType simpleType = FromValueType(t);

        //    if(t.BaseType != null)
        //    {
        //        simpleType.BaseType = FromComplexType(t.BaseType);
        //    }

        //    List<DbMetaProperty> props = new List<DbMetaProperty>();
        private DbMetaType CollectionType { get; set; }

        private DbMetaType BaseType { get; set; }

        public IMetaType TypeOf()
        {
            return this;
        }
    }
}