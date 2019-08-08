using Penguin.Reflection.Abstractions;
using Penguin.Reflection.Serialization.Constructors;
using Penguin.Reflection.Serialization.Objects;
using System.Collections.Generic;
using System.Data;

namespace Penguin.Persistence.Database.Serialization.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class DataTableExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        #region Methods

        /// <summary>
        /// Casts a DataTable into list of MetaObjects
        /// </summary>
        /// <param name="dt">The data table to be used as a source</param>
        /// <returns>A list of MetaObjects representing the data</returns>
        public static List<MetaObject> ToMetaObject(this DataTable dt)
        {
            MetaConstructor c = new MetaConstructor(new MetaConstructor.ConstructorSettings()
            {
                AttributeIncludeSettings = AttributeIncludeSettings.None
            });

            List<MetaObject> MetaRows = new List<MetaObject>();

            foreach (DataRow dr in dt.Rows)
            {
                MetaObject row = new MetaObject();

                foreach (DataColumn dc in dt.Columns)
                {
                    MetaObject item = new MetaObject();

                    MetaType objectType = MetaType.FromConstructor(c, dc.DataType);

                    item.Type = objectType;

                    item.Property = new MetaProperty()
                    {
                        Type = objectType,
                        Name = dc.ColumnName
                    };

                    item.Value = dr[dc]?.ToString();

                    row.AddProperty(item);
                }

                row.Type = new MetaType("SqlRow", row.Properties) { CoreType = CoreType.Reference };

                MetaRows.Add(row);
            }

            return MetaRows;
        }

        #endregion Methods
    }
}