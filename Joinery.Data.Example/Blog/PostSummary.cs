using System;
using Joinery.Data;

namespace Joinery.Data.Example.Blog
{
    public class PostSummary
    {
        // ---------------------------------------------------------------------------------------------
        // Public Static Methods
        // ---------------------------------------------------------------------------------------------

        public static Paged<PostSummary> SelectAllPosted(int page, int pageSize)
        {
            return Database
                .Select<PostSummary>()
                .Where(post => post.Posted == true)
                .OrderBy(post => post.PostedAt, descending: true)
                .ExecuteAllPaged(page, pageSize);
        }

        // ---------------------------------------------------------------------------------------------
        // Public Properties
        // ---------------------------------------------------------------------------------------------

        public int Comments
        {
            get;
            set;
        }

        public bool Posted
        {
            get;
            set;
        }

        public DateTime PostedAt
        {
            get;
            set;
        }

        public int PostId
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }
    }
}
