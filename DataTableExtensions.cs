using Penguin.Persistence.Database.Serialization.Extensions.Meta;
using Penguin.Reflection.Abstractions;
using Penguin.Reflection.Serialization.Abstractions.Wrappers;
using Penguin.Reflection.Serialization.Constructors;
using System.Collections.Generic;
using System.Data;

namespace Penguin.Persistence.Database.Serialization.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class DataTableExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Casts a DataTable into list of MetaObjects
        /// </summary>
        /// <param name="dt">The data table to be used as a source</param>
        /// <returns>A list of MetaObjects representing the data</returns>
        public static List<DbMetaObject> ToMetaObject(this DataTable dt)
        {
            if (dt is null)
            {
                throw new System.ArgumentNullException(nameof(dt));
            }

            List<DbMetaObject> MetaRows = new List<DbMetaObject>();

            foreach (DataRow dr in dt.Rows)
            {
                DbMetaObject row = new DbMetaObject();

                foreach (DataColumn dc in dt.Columns)
                {
                    DbMetaObject item = new DbMetaObject();

                    MetaTypeHolder objectType = new MetaTypeHolder(dc.DataType);

                    item.Type = objectType;

                    item.Property = new DbMetaProperty()
                    {
                        Type = objectType,
                        Name = dc.ColumnName
                    };

                    item.Value = dr[dc]?.ToString();

                    row.Properties.Add(item);
                }

                row.Type = new DbMetaType("SqlRow", row.Properties) { CoreType = CoreType.Reference };

                MetaRows.Add(row);
            }

            return MetaRows;
        }
    }
}