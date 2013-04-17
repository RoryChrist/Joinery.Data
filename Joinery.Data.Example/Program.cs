using System;
using Joinery.Data.Example.Blog;

namespace Joinery.Data.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var posts = Post.SelectAll();
            var comments = Comment.SelectAllGroupedByPost();

            foreach (var post in posts)
            {
                foreach (var comment in comments.SelectAll(post.PostId))
                {

                }
            }
        }
    }
}
