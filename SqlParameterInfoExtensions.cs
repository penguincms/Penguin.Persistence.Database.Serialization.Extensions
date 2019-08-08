using Penguin.Persistence.Abstractions.Attributes.Validation;
using Penguin.Reflection.Abstractions;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Constructors;
using Penguin.Reflection.Serialization.Objects;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace Penguin.Persistence.Database.Serialization.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class SqlParameterInfoExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        #region Methods

        /// <summary>
        /// Converts a SQL parameter into a MetaObject so that it can be serialized and displayed through a dynamic editor
        /// </summary>
        /// <param name="parameter">The parameter to convert</param>
        /// <param name="c">The optional MetaConstructor to use as a start, for caching</param>
        /// <returns>A Meta representation of the SQL parameter</returns>
        public static MetaObject ToMetaObject(this SQLParameterInfo parameter, MetaConstructor c = null)
        {
            c = c ?? new MetaConstructor(new MetaConstructor.ConstructorSettings()
            {
                AttributeIncludeSettings = AttributeIncludeSettings.All
            });

            System.Type PersistenceType = TypeConverter.ToNetType(parameter.DATA_TYPE);

            MetaType thisType = MetaType.FromConstructor(c, PersistenceType);

            MetaObject toReturn = new MetaObject()
            {
                Property = new MetaProperty()
                {
                    Name = parameter.PARAMETER_NAME,
                    Type = thisType
                },
                Type = thisType,
                Value = parameter.HAS_DEFAULT ? parameter.DEFAULT : null
            };

            if (PersistenceType == typeof(System.DateTime))
            {
                MetaAttribute rangeAttribute = new MetaAttribute(-1);
                rangeAttribute.Type = MetaType.FromConstructor(c, typeof(RangeAttribute));
                rangeAttribute.Instance = MetaObject.FromConstructor(c, new RangeAttribute(PersistenceType, SqlDateTime.MinValue.ToString(), SqlDateTime.MaxValue.ToString()));

                IList<IMetaAttribute> existingAttributes = toReturn.Property.Attributes.ToList();

                existingAttributes.Add(rangeAttribute);

                (toReturn.Property as MetaProperty).Attributes = existingAttributes;
            }

            return toReturn;
        }

        /// <summary>
        /// Converts a List of SQL parameters into MetaObjects so that they can be serialized and displayed through a dynamic editor
        /// </summary>
        /// <param name="parameters">A List of Sql parameters to serialize</param>
        /// <param name="c">The optional MetaConstructor to use as a start, for caching</param>
        /// <returns>A MetaObject representing a collection of the serialized parameters</returns>
        public static MetaObject ToMetaObject(this IEnumerable<SQLParameterInfo> parameters, MetaConstructor c = null)
        {
            MetaObject metaObject = new MetaObject();

            c = c ?? new MetaConstructor(new MetaConstructor.ConstructorSettings()
            {
                AttributeIncludeSettings = AttributeIncludeSettings.All
            });

            c.ClaimOwnership(metaObject);

            foreach (SQLParameterInfo thisParam in parameters)
            {
                MetaObject prop = thisParam.ToMetaObject(c);
                metaObject.AddProperty(prop);
            }

            metaObject.Type = new MetaType("SqlStoredProc", metaObject.Properties);
            metaObject.Type.CoreType = CoreType.Reference;

            metaObject.RegisterConstructor(c);

            return metaObject;
        }

        #endregion Methods
    }
}