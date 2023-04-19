namespace BlogWeb.Application.Entities
{
    public class PostMeta
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string Key { get; set; }
        public string Content { get; set; }
        public Post Post { get; set; }
    }
}