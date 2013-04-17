using System;
using Joinery.Data;

namespace Joinery.Data.Example.Blog
{
    public class Post
    {
        // ---------------------------------------------------------------------------------------------
        // Public Static Methods
        // ---------------------------------------------------------------------------------------------

        public static void IncrementComments(int postId)
        {
            Database
                .Update<Post>()
                .Set(post => post.Comments, post => post.Comments + 1)
                .Where(post => post.PostId == postId)
                .ExecuteOne();
        }

        public static void Save(Post post)
        {
            Database.Save(post);
        }

        public static Post[] SelectAll()
        {
            return Database
                .Select<Post>()
                .OrderBy(post => post.PostedAt, descending: true)
                .ExecuteAll();
        }

        public static Post SelectOne(int postId)
        {
            return Database
                .Select<Post>()
                .Where(post => post.PostId == postId)
                .ExecuteOne();
        }

        // ---------------------------------------------------------------------------------------------
        // Public Properties
        // ---------------------------------------------------------------------------------------------

        public string Body
        {
            get;
            set;
        }

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
