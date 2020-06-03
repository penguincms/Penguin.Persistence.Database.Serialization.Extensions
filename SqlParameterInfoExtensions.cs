using Penguin.Persistence.Abstractions.Attributes.Validation;
using Penguin.Persistence.Database.Serialization.Extensions.Meta;
using Penguin.Reflection.Abstractions;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Abstractions.Wrappers;
using Penguin.Reflection.Serialization.Constructors;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace Penguin.Persistence.Database.Serialization.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class SqlParameterInfoExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Converts a SQL parameter into a MetaObject so that it can be serialized and displayed through a dynamic editor
        /// </summary>
        /// <param name="parameter">The parameter to convert</param>
        /// <param name="c">The optional MetaConstructor to use as a start, for caching</param>
        /// <returns>A Meta representation of the SQL parameter</returns>
        public static DbMetaObject ToMetaObject(this SQLParameterInfo parameter, MetaConstructor c = null)
        {
            if (parameter is null)
            {
                throw new System.ArgumentNullException(nameof(parameter));
            }

            c = c ?? new MetaConstructor(new MetaConstructorSettings()
            {
                AttributeIncludeSettings = AttributeIncludeSetting.All
            });

            System.Type PersistenceType = TypeConverter.ToNetType(parameter.DATA_TYPE);

            IMetaType thisType = new MetaTypeHolder(PersistenceType);

            DbMetaObject toReturn = new DbMetaObject()
            {
                Property = new DbMetaProperty()
                {
                    Name = parameter.PARAMETER_NAME,
                    Type = thisType
                },
                Type = thisType,
                Value = parameter.HAS_DEFAULT ? parameter.DEFAULT : null
            };

            if (PersistenceType == typeof(System.DateTime))
            {
                IMetaAttribute rangeAttribute = new MetaAttributeHolder(new RangeAttribute(PersistenceType, SqlDateTime.MinValue.ToString(), SqlDateTime.MaxValue.ToString()), false);

                List<IMetaAttribute> existingAttributes = toReturn.Property.Attributes.ToList();

                existingAttributes.Add(rangeAttribute);

                toReturn.Property.Attributes = existingAttributes;
            }

            return toReturn;
        }

        /// <summary>
        /// Converts a List of SQL parameters into MetaObjects so that they can be serialized and displayed through a dynamic editor
        /// </summary>
        /// <param name="parameters">A List of Sql parameters to serialize</param>
        /// <param name="c">The optional MetaConstructor to use as a start, for caching</param>
        /// <returns>A MetaObject representing a collection of the serialized parameters</returns>
        public static DbMetaObject ToMetaObject(this IEnumerable<SQLParameterInfo> parameters, MetaConstructor c = null)
        {
            DbMetaObject metaObject = new DbMetaObject();

            c = c ?? new MetaConstructor(new MetaConstructorSettings()
            {
                AttributeIncludeSettings = AttributeIncludeSetting.All
            });

            c.ClaimOwnership(metaObject);

            foreach (SQLParameterInfo thisParam in parameters)
            {
                DbMetaObject prop = thisParam.ToMetaObject(c);
                metaObject.Properties.Add(prop);
            }

            metaObject.Type = new DbMetaType("SqlStoredProc", metaObject.Properties)
            {
                CoreType = CoreType.Reference
            };

            return metaObject;
        }
    }
}