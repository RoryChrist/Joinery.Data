using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;

namespace Joinery.Data
{
    internal static class Mapper
    {
        // ---------------------------------------------------------------------------------------------
        // Public Static Methods
        // ---------------------------------------------------------------------------------------------

        public static string GetColumnName(MemberInfo member)
        {
            var columnAttribute = Attribute.GetCustomAttribute(member, typeof(ColumnAttribute)) as ColumnAttribute;

            if (columnAttribute == null)
            {
                return member.Name;
            }
            else
            {
                return columnAttribute.Name;
            }
        }

        public static IEnumerable<PropertyInfo> GetColumnProperties(Type type, bool includeIdentityColumn)
        {
            var identityColumnPropertyName = GetIdentityColumnPropertyName(type);

            foreach (var property in type.GetProperties())
            {
                if (!(property.CanRead && property.CanWrite))
                {
                    continue;
                }

                if (!includeIdentityColumn && property.Name == identityColumnPropertyName)
                {
                    continue;
                }

                yield return property;
            }
        }

        public static string GetConnectionStringName(Type type)
        {
            return type.Namespace;
        }

        public static string GetIdentityColumnPropertyName(Type type)
        {
            return (type.Name + "Id");
        }

        public static int GetIdentityColumnValue(object item)
        {
            return (int)GetIdentityColumnProperty(item.GetType()).GetValue(item, null);
        }

        public static PropertyInfo GetIdentityColumnProperty(Type type)
        {
            var identityProperty = type.GetProperty(GetIdentityColumnPropertyName(type));

            if (identityProperty == null || identityProperty.PropertyType != typeof(int))
            {
                throw new InvalidOperationException("Identity column property not found.");
            }

            return identityProperty;
        }

        public static string GetTableName(Type type)
        {
            var tableAttribute = Attribute.GetCustomAttribute(type, typeof(TableAttribute), false) as TableAttribute;

            if (tableAttribute == null)
            {
                return type.Name;
            }
            else
            {
                return tableAttribute.Name;
            }
        }

        public static void SetColumnValues(object item, SqlDataReader reader)
        {
            foreach (var property in GetColumnProperties(item.GetType(), true))
            {
                object value = reader[GetColumnName(property)];

                if (value != DBNull.Value)
                {
                    property.SetValue(item, value, null);
                }
            }
        }

        public static void SetIdentityColumnValue(object item, int value)
        {
            GetIdentityColumnProperty(item.GetType()).SetValue(item, value, null);
        }
    }
}
