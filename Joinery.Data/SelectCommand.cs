using System;
using System.Linq.Expressions;
using System.Text;

namespace Joinery.Data
{
    public class SelectCommand<T> : CommandBase<T> where T : new()
    {
        // ---------------------------------------------------------------------------------------------
        // Public Methods
        // ---------------------------------------------------------------------------------------------

        public T[] ExecuteAll()
        {
            return Database.SelectAll<T>(PrepareSql(), Parameters.ToArray());
        }

        public T[] ExecuteAllCached()
        {
            return Database.SelectAllCached<T>(PrepareSql(), Parameters.ToArray());
        }

        public Grouped<T> ExecuteAllGroupedBy(Func<T, int> groupById)
        {
            return Database.SelectAllGroupedBy(PrepareSql(), groupById, Parameters.ToArray());
        }

        public Grouped<T> ExecuteAllGroupedByCached(Func<T, int> groupById)
        {
            return Database.SelectAllGroupedByCached(PrepareSql(), groupById, Parameters.ToArray());
        }

        public Paged<T> ExecuteAllPaged(int page)
        {
            return ExecuteAllPaged(page, Database.DefaultPageSize);
        }

        public Paged<T> ExecuteAllPaged(int page, int pageSize)
        {
            int count = ExecuteCount();

            if (count == 0)
            {
                return new Paged<T>(new T[0], 0, 0, pageSize, count);
            }

            int pages = (int)Math.Ceiling(count / (decimal)pageSize);

            if (page < 0)
            {
                page = 1;
            }
            else if (page > pages)
            {
                page = pages;
            }

            int fromRecord = ((page - 1) * pageSize) + 1;
            int throughRecord = fromRecord + pageSize - 1;

            StringBuilder wrapperSQL = new StringBuilder("SELECT * FROM (");

            wrapperSQL.Append(PrepareSql(includeOrderBy: false, includeRowNumber: true));

            wrapperSQL.Append(") NumberedRows WHERE RowNumber BETWEEN ");

            AppendParameter(wrapperSQL, fromRecord);

            wrapperSQL.Append(" AND ");

            AppendParameter(wrapperSQL, throughRecord);

            wrapperSQL.Append(" ORDER BY RowNumber");

            T[] items = Database.SelectAll<T>(wrapperSQL.ToString(), Parameters.ToArray());

            return new Paged<T>(items, page, pages, pageSize, count);
        }

        public int ExecuteCount()
        {
            return Database.ExecuteInt<T>(PrepareSql(includeColumns: false,
                                                    includeCount: true,
                                                    includeOrderBy: false),
                                                    Parameters.ToArray());
        }

        public T ExecuteFirst()
        {
            return Database.SelectFirst<T>(PrepareSql(), Parameters.ToArray());
        }

        public T ExecuteFirstCached()
        {
            return Database.SelectFirstCached<T>(PrepareSql(), Parameters.ToArray());
        }

        public T ExecuteOne()
        {
            return Database.SelectOne<T>(PrepareSql(), Parameters.ToArray());
        }

        public T ExecuteOneCached()
        {
            return Database.SelectOneCached<T>(PrepareSql(), Parameters.ToArray());
        }

        public SelectCommand<T> Join<J>(Expression<Func<J, T, bool>> predicate)
        {
            ParseJoin(predicate);

            return this;
        }

        public SelectCommand<T> OrderBy<R>(Expression<Func<T, R>> property, bool descending = false)
        {
            ParseOrderBy(property, descending);

            return this;
        }

        public SelectCommand<T> Where(Expression<Func<T, bool>> predicate)
        {
            ParseWhere(predicate);

            return this;
        }

        // ---------------------------------------------------------------------------------------------
        // Private Methods
        // ---------------------------------------------------------------------------------------------

        private string PrepareSql(bool includeColumns = true, bool includeCount = false, bool includeOrderBy = true, bool includeRowNumber = false)
        {
            StringBuilder sql = new StringBuilder("SELECT ");

            if (includeColumns)
            {
                AppendTable(sql, typeof(T));

                sql.Append(".*");
            }

            if (includeCount)
            {
                sql.Append("COUNT(*)");
            }

            if (includeRowNumber)
            {
                sql.Append(", ROW_NUMBER() OVER (ORDER BY ");
                sql.Append(OrderbyClause);
                sql.Append(") AS RowNumber");
            }

            sql.Append(" FROM ");

            AppendTable(sql, typeof(T));

            if (JoinClause != null)
            {
                sql.Append(" INNER JOIN ");
                sql.Append(JoinClause);
            }

            if (WhereClause != null)
            {
                sql.Append(" WHERE ");
                sql.Append(WhereClause);
            }

            if (OrderbyClause != null && includeOrderBy)
            {
                sql.Append(" ORDER BY ");
                sql.Append(OrderbyClause);
            }

            return sql.ToString();
        }
    }
}