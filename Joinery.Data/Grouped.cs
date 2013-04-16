using System;
using System.Collections;
using System.Collections.Generic;

namespace Joinery.Data
{
    public class Grouped<T>
    {
        // ---------------------------------------------------------------------------------------------
        // Private Fields
        // ---------------------------------------------------------------------------------------------

        private List<int> groupIds;
        private Dictionary<int, List<T>> groups;

        // ---------------------------------------------------------------------------------------------
        // Public Constructors
        // ---------------------------------------------------------------------------------------------

        public Grouped()
        {
            groupIds = new List<int>();
            groups = new Dictionary<int, List<T>>();
        }

        public Grouped(IEnumerable<T> items, Func<T, int> groupId) : this()
        {
            foreach (T item in items)
            {
                Add(item, groupId(item));
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Public Properties
        // ---------------------------------------------------------------------------------------------

        public int Count
        {
            get
            {
                return groups.Count;
            }
        }

        public IEnumerable<int> GroupIds
        {
            get { return groupIds; }
        }

        // ---------------------------------------------------------------------------------------------
        // Public Methods
        // ---------------------------------------------------------------------------------------------

        public void Add(T item, int groupId)
        {
            List<T> group;

            if (groups.ContainsKey(groupId))
            {
                group = groups[groupId];
            }
            else
            {
                group = new List<T>();

                groups.Add(groupId, group);

                groupIds.Add(groupId);
            }

            group.Add(item);
        }

        public bool Contains(int groupId)
        {
            return groups.ContainsKey(groupId);
        }

        public T[] FindAll(int groupId)
        {
            List<T> group;

            if (groups.TryGetValue(groupId, out group))
            {
                return group.ToArray();
            }
            else
            {
                return new T[0];
            }
        }

        public T FindFirst(int groupId)
        {
            List<T> group;

            if (groups.TryGetValue(groupId, out group) && group.Count > 0)
            {
                return group[0];
            }
            else
            {
                return default(T);
            }
        }

        public T FindOne(int groupId)
        {
            List<T> group;

            if (groups.TryGetValue(groupId, out group) && group.Count == 1)
            {
                return group[0];
            }
            else
            {
                throw new NotFoundException();
            }
        }

        public bool TryFindOne(int groupId, out T item)
        {
            List<T> group;

            if (groups.TryGetValue(groupId, out group) && group.Count == 1)
            {
                item = group[0];

                return true;
            }
            else
            {
                item = default(T);

                return false;
            }
        }
    }
}
