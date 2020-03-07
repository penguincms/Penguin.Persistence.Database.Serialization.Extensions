using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Penguin.Persistence.Database.Serialization.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class MetaObjectExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        #region Methods

        /// <summary>
        /// Converts a MetaObject containing a collection of SQL parameter definitions to concrete parameters
        /// </summary>
        /// <param name="metaObject">The meta object containing the collection</param>
        /// <returns>A collection of SQL parameter definitions to concrete parameters</returns>
        public static List<SqlParameter> ToSqlParameters(this IMetaObject metaObject)
        {
            List<SqlParameter> toReturn = new List<SqlParameter>();

            foreach (IMetaObject thisProperty in metaObject.Properties)
            {
                SqlParameter thisparam = new SqlParameter
                {
                    Value = thisProperty.Value,

                    ParameterName = thisProperty.Property.Name
                };

                toReturn.Add(thisparam);
            }

            return toReturn;
        }

        #endregion Methods
    }
}