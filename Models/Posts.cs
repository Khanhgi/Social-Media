namespace Social_Media.Models
{
    public class Posts
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public DateTime PostedAt { get; set; } = DateTime.Now;
    }
}
