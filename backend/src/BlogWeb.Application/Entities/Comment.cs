namespace BlogWeb.Application.Entities
{
    public class Comment : BaseEntity
    {
        public string CommentContents { get; set; }
        public string PostedBy { get; set; }
        public Comment ParentComment { get; set; }
        public int? ParentCommentId { get; set; }
        public virtual ICollection<Comment> ChildComments { get; } = new HashSet<Comment>();
        public Post ParentPost { get; set; }
        public int PostId { get; set; }
    }
}