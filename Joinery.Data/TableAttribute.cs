using System;

namespace Joinery.Data
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        // ---------------------------------------------------------------------------------------------
        // Public Constructor
        // ---------------------------------------------------------------------------------------------

        public TableAttribute(string name)
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
