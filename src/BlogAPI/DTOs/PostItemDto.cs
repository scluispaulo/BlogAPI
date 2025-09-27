public record PostItemDto
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public int CommentCount { get; init; }
}
