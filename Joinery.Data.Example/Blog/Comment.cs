using System;
using Joinery.Data;

namespace Joinery.Data.Example.Blog
{
    public class Comment
    {
        // ---------------------------------------------------------------------------------------------
        // Public Static Methods
        // ---------------------------------------------------------------------------------------------

        public static void Delete(Comment comment)
        {
            Database.Delete(comment);
        }

        public static void DeleteAllForPost(int postId)
        {
            Database
                .Delete<Comment>()
                .Where(comment => comment.PostId == postId)
                .ExecuteAll();
        }

        public static void Save(Comment comment)
        {
            Database.Save(comment);
        }

        public static Comment[] SelectAllForPost(int postId)
        {
            return Database
                .Select<Comment>()
                .Where(comment => comment.PostId == postId)
                .OrderBy(comment => comment.CreatedAt)
                .ExecuteAll();
        }

        // ---------------------------------------------------------------------------------------------
        // Public Properties
        // ---------------------------------------------------------------------------------------------

        public string Author
        {
            get;
            set;
        }

        public string Body
        {
            get;
            set;
        }

        public int CommentId
        {
            get;
            set;
        }

        public DateTime CreatedAt
        {
            get;
            set;
        }

        public int PostId
        {
            get;
            set;
        }
    }
}
