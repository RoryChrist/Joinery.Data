using System;
using System.Collections;
using System.Collections.Generic;

namespace Joinery.Data
{
    public interface Paged
    {
        int Page
        {
            get;
        }

        int Pages
        {
            get;
        }

        int PageSize
        {
            get;
        }
    }

    public class Paged<T> : Paged, IEnumerable<T>
    {
        // ---------------------------------------------------------------------------------------------
        // Private Fields
        // ---------------------------------------------------------------------------------------------

        private IList<T> items;

        // ---------------------------------------------------------------------------------------------
        // Public Constructor
        // ---------------------------------------------------------------------------------------------

        public Paged(IList<T> items, int page, int pages, int pageSize, int totalCount)
        {
            this.items = items;

            Page = page;
            Pages = pages;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        // ---------------------------------------------------------------------------------------------
        // Public Properties
        // ---------------------------------------------------------------------------------------------

        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        public int Page
        {
            get;
            private set;
        }

        public int Pages
        {
            get;
            private set;
        }

        public int PageSize
        {
            get;
            private set;
        }

        public T this[int index]
        {
            get
            {
                return items[index];
            }
        }

        public int TotalCount
        {
            get;
            private set;
        }

        // ---------------------------------------------------------------------------------------------
        // IEnumerable<T> Methods
        // ---------------------------------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}