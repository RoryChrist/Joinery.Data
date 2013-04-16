using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Joinery.Data
{
    internal static class Executer
    {
        // ---------------------------------------------------------------------------------------------
        // Public Static Methods
        // ---------------------------------------------------------------------------------------------

        public static int ExecuteNonQuery<T>(string sql, object[] parameters)
        {
            using (var connection = new SqlConnection(GetConnectionString<T>()))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();

                AddParameters(command, parameters);

                return command.ExecuteNonQuery();
            }
        }

        public static IEnumerable<T> ExecuteReader<T>(string sql, object[] parameters) where T : new()
        {
            using (var connection = new SqlConnection(GetConnectionString<T>()))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();

                AddParameters(command, parameters);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T item = new T();

                        Mapper.SetColumnValues(item, reader);

                        yield return item;
                    }
                }
            }
        }

        public static object ExecuteScalar<T>(string sql, object[] parameters)
        {
            using (var connection = new SqlConnection(GetConnectionString<T>()))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();

                AddParameters(command, parameters);

                return command.ExecuteScalar();
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Private Static Methods
        // ---------------------------------------------------------------------------------------------

        private static void AddParameters(SqlCommand command, object[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                command.Parameters.Add(new SqlParameter("@P" + i.ToString(), parameters[i] ?? DBNull.Value));
            }
        }

        private static string GetConnectionString<T>()
        {
            var connectionStringName = Mapper.GetConnectionStringName(typeof(T));

            var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (connectionStringSettings == null)
            {
                throw new Exception(string.Format("Couldn't find connection string named '{0}' in config file.", connectionStringName));
            }

            return connectionStringSettings.ConnectionString;
        }
    }
}
