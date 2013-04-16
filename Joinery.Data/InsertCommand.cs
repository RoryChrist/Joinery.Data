using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Joinery.Data
{
    public class InsertCommand<T> : CommandBase<T>
    {
        // ---------------------------------------------------------------------------------------------
        // Public Static Methods
        // ---------------------------------------------------------------------------------------------

        public static void ExecuteOne(T item)
        {
            InsertCommand<T> insertCommand = new InsertCommand<T>();

            foreach (PropertyInfo property in Mapper.GetColumnProperties(typeof(T), false))
            {
                insertCommand.AppendInsertColumnValue(property, property.GetValue(item, null));
            }

            int identity = insertCommand.ExecuteSelectIdentity();

            Mapper.SetIdentityColumnValue(item, identity);
        }

        // ---------------------------------------------------------------------------------------------
        // Public Methods
        // ---------------------------------------------------------------------------------------------

        public int ExecuteSelectIdentity()
        {
            return Database.ExecuteInt<T>(PrepareSql(true), Parameters.ToArray());
        }

        public void ExecuteOne()
        {
            Database.ExecuteOne<T>(PrepareSql(false), Parameters.ToArray());
        }

        public InsertCommand<T> Value<R>(Expression<Func<T, R>> property, R value)
        {
            ParseValue(property, value);

            return this;
        }

        // ---------------------------------------------------------------------------------------------
        // Private Methods
        // ---------------------------------------------------------------------------------------------

        private string PrepareSql(bool selectIdentity)
        {
            StringBuilder sql = new StringBuilder("INSERT INTO ");

            AppendTable(sql, typeof(T));

            if (ColumnList != null)
            {
                sql.Append(" (");
                sql.Append(ColumnList);
                sql.Append(")");
            }

            if (ValueList != null)
            {
                sql.Append(" VALUES (");
                sql.Append(ValueList);
                sql.Append(")");
            }

            if (selectIdentity)
            {
                sql.Append("; SELECT SCOPE_IDENTITY()");
            }

            return sql.ToString();
        }
    }
}