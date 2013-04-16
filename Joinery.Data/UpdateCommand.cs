using System;
using System.Linq.Expressions;
using System.Text;

namespace Joinery.Data
{
    public class UpdateCommand<T> : CommandBase<T>
    {
        // ---------------------------------------------------------------------------------------------
        // Public Static Methods
        // ---------------------------------------------------------------------------------------------

        public static void ExecuteOne(T item)
        {
            var updateCommand = new UpdateCommand<T>();

            foreach (var property in Mapper.GetColumnProperties(typeof(T), false))
            {
                updateCommand.AppendUpdateColumnValue(property, property.GetValue(item, null));
            }

            updateCommand.AppendWhereCondition(Mapper.GetIdentityColumnProperty(typeof(T)), Mapper.GetIdentityColumnValue(item));

            updateCommand.ExecuteOne();
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

        public UpdateCommand<T> Set<R>(Expression<Func<T, R>> property, R value)
        {
            ParseSet(property, value);

            return this;
        }

        public UpdateCommand<T> Set<R>(Expression<Func<T, R>> property, Expression<Func<T, R>> value)
        {
            ParseSet(property, value);

            return this;
        }

        public UpdateCommand<T> Where(Expression<Func<T, bool>> predicate)
        {
            ParseWhere(predicate);

            return this;
        }

        // ---------------------------------------------------------------------------------------------
        // Private Methods
        // ---------------------------------------------------------------------------------------------

        private string PrepareSql()
        {
            StringBuilder sql = new StringBuilder("UPDATE ");

            AppendTable(sql, typeof(T));

            if (SetList != null)
            {
                sql.Append(" SET ");
                sql.Append(SetList);
            }

            if (WhereClause != null)
            {
                sql.Append(" WHERE ");
                sql.Append(WhereClause);
            }

            return sql.ToString();
        }
    }
}