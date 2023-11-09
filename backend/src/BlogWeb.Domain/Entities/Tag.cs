namespace BlogWeb.Domain.Entities
{
    public class Tag : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public ICollection<PostTags> PostTags { get; set; } = new List<PostTags>();
    }
}