using System;
using System.Collections.Generic;
using System.Linq;

namespace Joinery.Data
{
    public static class Database
    {
        // ---------------------------------------------------------------------------------------------
        // Public Constants
        // ---------------------------------------------------------------------------------------------

        public const int DefaultCacheExpirationSeconds = 10;
        public const int DefaultPageSize = 50;

        // ---------------------------------------------------------------------------------------------
        // Public Static Methods
        // ---------------------------------------------------------------------------------------------

        public static DeleteCommand<T> Delete<T>()
        {
            return new DeleteCommand<T>();
        }

        public static void Delete<T>(T item)
        {
            DeleteCommand<T>.ExecuteOne(item);
        }

        public static int ExecuteAll<T>(string sql, params object[] parameters)
        {
            return Executer.ExecuteNonQuery<T>(sql, parameters);
        }

        public static int ExecuteInt<T>(string sql, params object[] parameters)
        {
            return Convert.ToInt32(Executer.ExecuteScalar<T>(sql, parameters));
        }

        public static void ExecuteOne<T>(string sql, params object[] parameters)
        {
            int count = Executer.ExecuteNonQuery<T>(sql, parameters);

            if (count != 1)
            {
                throw new Exception(string.Format("Exception executing SQL command. {0} records were affected but only one was expected.", count));
            }
        }

        public static InsertCommand<T> Insert<T>()
        {
            return new InsertCommand<T>();
        }

        public static void Insert<T>(T item)
        {
            InsertCommand<T>.ExecuteOne(item);
        }

        public static bool IsNew<T>(T item)
        {
            return (Mapper.GetIdentityColumnValue(item) == default(int));
        }

        public static void Save<T>(T item)
        {
            if (IsNew(item))
            {
                Insert(item);
            }
            else
            {
                Update(item);
            }
        }

        public static SelectCommand<T> Select<T>() where T : new()
        {
            return new SelectCommand<T>();
        }

        public static T[] SelectAll<T>(string sql, params object[] parameters) where T : new()
        {
            return Executer.ExecuteReader<T>(sql, parameters).ToArray();
        }

        public static T[] SelectAllCached<T>(string sql, params object[] parameters) where T : new()
        {
            return Cache.Get<T[]>(sql, parameters, () => SelectAll<T>(sql, parameters));
        }

        public static Grouped<T> SelectAllGroupedBy<T>(string sql, Func<T, int> groupId, params object[] parameters) where T : new()
        {
            return new Grouped<T>(Executer.ExecuteReader<T>(sql, parameters), groupId);
        }

        public static Grouped<T> SelectAllGroupedByCached<T>(string sql, Func<T, int> groupId, params object[] parameters) where T : new()
        {
            return Cache.Get<Grouped<T>>(sql, parameters, () => SelectAllGroupedBy<T>(sql, groupId, parameters));
        }

        public static Paged<T> SelectAllPaged<T>(int page, string sql, string orderBy, params object[] parameters) where T : new()
        {
            return SelectAllPaged<T>(page, DefaultPageSize, sql, orderBy, parameters);
        }

        public static Paged<T> SelectAllPaged<T>(int page, int pageSize, string sql, string orderBy, params object[] parameters) where T : new()
        {
            throw new NotImplementedException();
        }

        public static T SelectFirst<T>(string sql, params object[] parameters) where T : new()
        {
            return Executer.ExecuteReader<T>(sql, parameters).FirstOrDefault();
        }

        public static T SelectFirstCached<T>(string sql, params object[] parameters) where T : new()
        {
            return Cache.Get<T>(sql, parameters, () => SelectFirst<T>(sql, parameters));
        }

        public static T SelectOne<T>(string sql, params object[] parameters) where T : new()
        {
            try
            {
                return Executer.ExecuteReader<T>(sql, parameters).Single();
            }
            catch (InvalidOperationException)
            {
                throw new NotFoundException();
            }
        }

        public static T SelectOneCached<T>(string sql, params object[] parameters) where T : new()
        {
            return Cache.Get<T>(sql, parameters, () => SelectOne<T>(sql, parameters));
        }

        public static UpdateCommand<T> Update<T>()
        {
            return new UpdateCommand<T>();
        }

        public static void Update<T>(T item)
        {
            UpdateCommand<T>.ExecuteOne(item);
        }
    }
}
