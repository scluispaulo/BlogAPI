public class BlogPost
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
