using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Joinery.Data
{
    public class DeleteCommand<T> : CommandBase<T>
    {
        // ---------------------------------------------------------------------------------------------
        // Public Static Methods
        // ---------------------------------------------------------------------------------------------

        public static void ExecuteOne(T item)
        {
            DeleteCommand<T> deleteCommand = new DeleteCommand<T>();

            deleteCommand.AppendWhereCondition(Mapper.GetIdentityColumnProperty(typeof(T)), Mapper.GetIdentityColumnValue(item));

            deleteCommand.ExecuteOne();
        }

        // ---------------------------------------------------------------------------------------------
        // Public Methods
        // ---------------------------------------------------------------------------------------------

        public int ExecuteAll()
        {
            return Database.ExecuteAll<T>(PrepareSql(), Parameters.ToArray());
        }

        public void ExecuteOne()
        {
            Database.ExecuteOne<T>(PrepareSql(), Parameters.ToArray());
        }

        public DeleteCommand<T> Where(Expression<Func<T, bool>> predicate)
        {
            ParseWhere(predicate);

            return this;
        }

        // ---------------------------------------------------------------------------------------------
        // Private Methods
        // ---------------------------------------------------------------------------------------------

        private string PrepareSql()
        {
            StringBuilder sql = new StringBuilder("DELETE FROM ");

            AppendTable(sql, typeof(T));

            if (WhereClause != null)
            {
                sql.Append(" WHERE ");
                sql.Append(WhereClause);
            }

            return sql.ToString();
        }
    }
}