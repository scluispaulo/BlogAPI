public static class CommentEndpoints
{
    public static void MapCommentEndpoints(this WebApplication app)
    {
        app.MapPost("/api/posts/{id:int}/comments", async (int id, CreateCommentDto input, BlogContext db) =>
        {
            var post = await db.BlogPosts.FindAsync(id);
            if (post is null)
                return Results.NotFound();

            var comment = new Comment
            {
                BlogPostId = id,
                Author = input.Author.Trim(),
                Content = input.Content.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            db.Comments.Add(comment);
            await db.SaveChangesAsync();

            var dto = new CommentDto
            {
                Id = comment.Id,
                Author = comment.Author,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt
            };

            return Results.Created($"/api/posts/{id}/comments/{comment.Id}", dto);
        });
    }
}
