using System;
using System.Runtime.Caching;

namespace Joinery.Data
{
    internal static class Cache
    {
        // ---------------------------------------------------------------------------------------------
        // Public Constants
        // ---------------------------------------------------------------------------------------------

        public const int DefaultCacheExpirationSeconds = 10;

        // ---------------------------------------------------------------------------------------------
        // Public Static Methods
        // ---------------------------------------------------------------------------------------------

        public static T Get<T>(string sql, object[] parameters, Func<T> command)
        {
            string key = GetKey(sql, parameters);

            T result = (T)MemoryCache.Default.Get(key, null);

            if (result == null)
            {
                result = command();

                if (result != null)
                {
                    var expires = DateTimeOffset.Now.AddSeconds(DefaultCacheExpirationSeconds);

                    MemoryCache.Default.Set(key, result, expires, null);
                }
            }

            return result;
        }

        // ---------------------------------------------------------------------------------------------
        // Private Static Methods
        // ---------------------------------------------------------------------------------------------

        private static string GetKey(string sql, object[] parameters)
        {
            var parts = new string[parameters.Length + 1];

            parts[0] = sql;

            for (int i = 0; i < parameters.Length; i++)
            {
                parts[i + 1] = parameters[i].ToString();
            }

            return string.Join("~", parts);
        }
    }
}
