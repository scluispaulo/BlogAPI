using System.Text.Json;
using BlogAPI.Helpers;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

public static class PostEndpoints
{
    public static void MapPostEndpoints(this WebApplication app)
    {
       var apiPosts = app.MapGroup("/api/posts");

        apiPosts.MapGet("/", async (BlogContext db, HttpContext httpContext, int page = 1, int pageSize = 10) =>
        {
            var query = db.BlogPosts.Include(p => p.Comments).OrderBy(p => p.Id);

            var paginated = await PaginatedList<BlogPost>.CreateAsync(query, page, pageSize);

            var postDtos = paginated.Select(p => new PostItemDto
            {
                Id = p.Id,
                Title = p.Title,
                CommentCount = p.Comments.Count
            }).ToArray();

            var paginatedMetadata = new
            {
                paginated.CurrentPage,
                paginated.PageSize,
                paginated.TotalCount,
                paginated.TotalPages,
                Items = postDtos
            };

            httpContext.Response.Headers["X-Pagination"] = JsonSerializer.Serialize(paginatedMetadata);

            return Results.Ok(paginatedMetadata);
        });

        apiPosts.MapGet("/{id:int}", async (int id, BlogContext db) =>
        {
            var post = await db.BlogPosts
                .Include(p => p.Comments.OrderBy(c => c.CreatedAt))
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post is null)
                return Results.NotFound();

            var dto = new PostWithCommentsDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                Comments = post.Comments.Select(c =>
                    new CommentDto
                    {
                        Id = c.Id,
                        Author = c.Author,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt
                    }
                ).ToList()
            };

            return Results.Ok(dto);
        });

        apiPosts.MapPost("/", async (
            CreatePostDto input,
            BlogContext db,
            IValidator<CreatePostDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(input);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            var post = new BlogPost
            {
                Title = input.Title.Trim(),
                Content = input.Content.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            db.BlogPosts.Add(post);
            await db.SaveChangesAsync();

            var result = new PostWithCommentsDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                Comments = new List<CommentDto>()
            };

            return Results.Created($"/api/posts/{post.Id}", result);
        });
    }
}