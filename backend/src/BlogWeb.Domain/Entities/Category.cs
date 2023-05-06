namespace BlogWeb.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public ICollection<Post> AssociatedPosts { get; set; } = new List<Post>();
        public List<PostCategories> PostCategories { get; set; } = new List<PostCategories>();
    }
}