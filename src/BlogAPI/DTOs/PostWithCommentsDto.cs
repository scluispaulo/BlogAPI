public class PostWithCommentsDto
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public string Content { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public List<CommentDto> Comments { get; init; } = new();
}