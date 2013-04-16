using System;

namespace Joinery.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        // ---------------------------------------------------------------------------------------------
        // Public Constructor
        // ---------------------------------------------------------------------------------------------

        public ColumnAttribute(string name)
        {
            Name = name;
        }

        // ---------------------------------------------------------------------------------------------
        // Public Properties
        // ---------------------------------------------------------------------------------------------

        public string Name
        {
            get;
            private set;
        }
    }
}
