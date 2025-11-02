using System.Text.Json;
using BlogAPI.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly BlogContext _db;
    private readonly IValidator<CreatePostDto> _validator;

    public PostsController(BlogContext db, IValidator<CreatePostDto> validator)
    {
        _db = db;
        _validator = validator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = _db.BlogPosts.Include(p => p.Comments).OrderBy(p => p.Id);

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

        Response.Headers["X-Pagination"] = JsonSerializer.Serialize(paginatedMetadata);

        return Ok(paginatedMetadata);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var post = await _db.BlogPosts
            .Include(p => p.Comments.OrderBy(c => c.CreatedAt))
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post is null)
            return NotFound();

        var dto = new PostWithCommentsDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
            Comments = post.Comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Author = c.Author,
                Content = c.Content,
                CreatedAt = c.CreatedAt
            }).ToList()
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostDto input)
    {
        var validationResult = await _validator.ValidateAsync(input);
        if (!validationResult.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));

        var post = new BlogPost
        {
            Title = input.Title.Trim(),
            Content = input.Content.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.BlogPosts.Add(post);
        await _db.SaveChangesAsync();

        var result = new PostWithCommentsDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
            Comments = new List<CommentDto>()
        };

        return CreatedAtAction(nameof(GetById), new { id = post.Id }, result);
    }
}
