CREATE TABLE Comment
(
    Author      NVARCHAR(100)       NOT NULL,
    Body        NVARCHAR(MAX)       NOT NULL,
    CommentId   INT IDENTITY(1,1)   NOT NULL,
    CreatedAt   DATETIME            NOT NULL,
    PostId      INT                 NOT NULL    
)
ALTER TABLE Comment ADD CONSTRAINT PK_Comment PRIMARY KEY (CommentId)

CREATE TABLE Post
(
    Body        NVARCHAR(MAX)       NOT NULL,
    Comments    INT                 NOT NULL,
    Posted      BIT                 NOT NULL,
    PostedAt    DATETIME            NOT NULL,
    PostId      INT IDENTITY(1,1)   NOT NULL,
    Title       NVARCHAR(100)       NOT NULL
)
ALTER TABLE Post ADD CONSTRAINT PK_Post PRIMARY KEY (PostId)
GO

ALTER TABLE Comment ADD CONSTRAINT FK_Comment_PostId FOREIGN KEY (PostId) REFERENCES Post (PostId)
GO

CREATE VIEW PostSummary
AS
    SELECT
        Post.Comments,
        Post.Posted,
        Post.PostedAt,
        Post.PostId,
        Post.Title
    FROM
        Post
GO