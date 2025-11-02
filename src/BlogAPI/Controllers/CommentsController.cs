using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;

[ApiController]
[Route("api/posts/{postId:int}/comments")]
public class CommentsController : ControllerBase
{
    private readonly BlogContext _db;
    private readonly IValidator<CreateCommentDto> _validator;

    public CommentsController(BlogContext db, IValidator<CreateCommentDto> validator)
    {
        _db = db;
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(int postId, [FromBody] CreateCommentDto input)
    {
        var validationResult = await _validator.ValidateAsync(input);
        if (!validationResult.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));

        var post = await _db.BlogPosts.FindAsync(postId);
        if (post == null)
            return NotFound();

        var comment = new Comment
        {
            BlogPostId = postId,
            Author = input.Author.Trim(),
            Content = input.Content.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        var dto = new CommentDto
        {
            Id = comment.Id,
            Author = comment.Author,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt
        };

        return CreatedAtAction(nameof(AddComment), new { postId, id = comment.Id }, dto);
    }
}
