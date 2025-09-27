public class CommentDto
{
    public int Id { get; init; }
    public string Author { get; init; } = null!;
    public string Content { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
}