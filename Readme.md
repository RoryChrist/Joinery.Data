# Joinery.Data

Joinery.Data is a lightweight data access library for .NET and SQL Server, featuring:

- LINQ expression support for basic SELECT, INSERT, UPDATE and DELETE statements
- SQL support for complex queries
- Low complexity for ease of debugging and modification
- Minimal configuration

## Querying

### Querying with LINQ

Selecting an array of records:

    public static Comment[] SelectAllForPost(int postId)
    {
        return Database
            .Select<Comment>()
            .Where(comment => comment.PostId == postId)
            .OrderBy(comment => comment.CreatedAt)
            .ExecuteAll();
    }

Selecting a single record (throws a `NotFoundException` if zero or multiple records are returned):

    public static Post SelectOne(int postId)
    {
        return Database
            .Select<Post>()
            .Where(post => post.PostId == postId)
            .ExecuteOne();
    }

Select a page of records:

    public static Paged<PostSummary> SelectAllPosted(int page, int pageSize)
    {
        return Database
            .Select<PostSummary>()
            .Where(post => post.Posted == true)
            .OrderBy(post => post.PostedAt, descending: true)
            .ExecuteAllPaged(page, pageSize);
    }

Select parent-child records:

    public static Post[] SelectAll()
    {
        return Database
            .Select<Post>()
            .OrderBy(post => post.PostedAt, descending: true)
            .ExecuteAll();
    }

    public static Grouped<Comment> SelectAllGroupedByPost()
    {
        return Database
            .Select<Comment>()
            .OrderBy(comment => comment.CreatedAt, descending: true)
            .ExecuteAllGroupedBy(comment => comment.PostId);
    }

    var posts = Post.SelectAll();
    var comments = Comment.SelectAllGroupedByPost();

    foreach (var post in posts)
    {
        foreach (var comment in comments.SelectAll(post.PostId))
        {
        }
    }

## Modifying Data

### Working with Entities

Deleting an existing entity:

    public static void Delete(Comment comment)
    {
        Database.Delete(comment);
    }

Saving a new or existing entity:

    public static void Save(Comment comment)
    {
        Database.Save(comment);
    }

### Modifying Data with LINQ

Deleting by foreign key:

    public static void DeleteAllForPost(int postId)
    {
        Database
            .Delete<Comment>()
            .Where(comment => comment.PostId == postId)
            .ExecuteAll();
    }

Updating with an expression:

    public static void IncrementComments(int postId)
    {
        Database
            .Update<Post>()
            .Set(post => post.Comments, post => post.Comments + 1)
            .Where(post => post.PostId == postId)
            .ExecuteOne();
    }

## Configuration

### Mapping Conventions

- Class for each table/view
- Table/class names are singular
- Property for each column
- Primary key is TableName + "Id"

### Mapping Attributes

The default mapping conventions can be overridden with the `TableAttribute` and `ColumnAttribute` for cases where you don't have control over the database schema:

    [Table("Posts")]
    public class Post
    {
        [Column("Id")]
        public int PostId
        {
            get;
            set;
        }
    }

### Connection String

The connection string name should match the namespace of the entity classes:

    <connectionStrings>
        <add name="Joinery.Data.Example.Blog" connectionString="Server=(local)\SQL2012; Database=Example; Trusted_Connection=true;" />
    </connectionStrings>


