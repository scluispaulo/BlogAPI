public class Comment
{
    public int Id { get; set; }
    public int BlogPostId { get; set; }
    public string Author { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    
    public BlogPost BlogPost { get; set; } = null!;
}
