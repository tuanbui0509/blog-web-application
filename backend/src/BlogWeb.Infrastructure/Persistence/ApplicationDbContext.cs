using BlogWeb.Application.Interfaces.Repositories;
using BlogWeb.Domain.Entities;
using BlogWeb.Domain.Entities.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BlogWeb.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<UserApplication>
    {
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
                                    ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        #region Add DbSet
        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PostCategories> PostCategories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTags> PostTags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostMeta> PostMetas { get; set; }
        #endregion
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (EntityEntry<BaseEntity> entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedBy = _currentUserService.UserId;
                        entry.Entity.ModifiedAt = DateTime.UtcNow;
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            // await DispatchEvents();

            return result;
        }
        // private async Task DispatchEvents()
        // {
        //     while (true)
        //     {
        //         var domainEventEntity = ChangeTracker
        //             .Entries<IHasDomainEvent>()
        //             .Select(x => x.Entity.DomainEvents)
        //             .SelectMany(x => x)
        //             .FirstOrDefault(domainEvent => !domainEvent.IsPublished);

        //         if (domainEventEntity == null) break;

        //         domainEventEntity.IsPublished = true;
        //         await _domainEventService.Publish(domainEventEntity);
        //     }
        // }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Category
            modelBuilder.Entity<Category>(p =>
            {
                p.ToTable("Categories");
                p.HasKey(p => p.Id);
                p.Property(e => e.Id).UseIdentityColumn();
                p.Property(e => e.CreatedAt).HasDefaultValueSql("getutcdate()");
                p.Property(e => e.ModifiedAt).HasDefaultValueSql("getutcdate()");
                p.Property(e => e.Title).IsRequired().HasMaxLength(150);
                p.Property(e => e.Slug).IsRequired().HasMaxLength(100);
                p.Property(e => e.Description).IsRequired().HasMaxLength(300);
            });
            //Post
            modelBuilder.Entity<Post>(p =>
            {
                p.ToTable("Posts");
                p.HasKey(p => p.Id);
                p.Property(e => e.Id).UseIdentityColumn();
                p.Property(e => e.CreatedAt).HasDefaultValueSql("getutcdate()");
                p.Property(e => e.ModifiedAt).HasDefaultValueSql("getutcdate()");
                p.Property(e => e.Title).IsRequired().HasMaxLength(150);
                p.Property(e => e.Slug).IsRequired().HasMaxLength(100);
                p.Property(e => e.Summary).IsRequired().HasMaxLength(150);
                p.Property(e => e.PostContents).IsRequired().HasMaxLength(300);
                p.HasOne(e => e.UserApplication)
                        .WithMany(e => e.Posts)
                        .HasForeignKey(e => e.AuthorId)
                        .HasConstraintName("FK_Post_User");
            });

            // Tags
            modelBuilder.Entity<Tag>(p =>
            {
                p.ToTable("Tags");
                p.HasKey(p => p.Id);
                p.Property(e => e.Id).UseIdentityColumn();
                p.Property(e => e.CreatedAt).HasDefaultValueSql("getutcdate()");
                p.Property(e => e.ModifiedAt).HasDefaultValueSql("getutcdate()");
                p.Property(e => e.Title).IsRequired().HasMaxLength(150);
                p.Property(e => e.Slug).IsRequired().HasMaxLength(100);
                p.Property(e => e.Description).IsRequired().HasMaxLength(250);
            });
            // PostCategories
            modelBuilder.Entity<PostCategories>(p =>
            {
                p.ToTable("PostCategories");
                p.HasKey(p => p.Id);
                p.Property(e => e.Id).UseIdentityColumn();
                p.HasOne(e => e.Post)
                        .WithMany(e => e.PostCategories)
                        .HasForeignKey(e => e.PostId)
                        .HasConstraintName("FK_PostCategories_Post");

                p.HasOne(e => e.Category)
                        .WithMany(e => e.PostCategories)
                        .HasForeignKey(e => e.CategoryId)
                        .HasConstraintName("FK_PostCategories_Category");
            });
            // PostTags
            modelBuilder.Entity<PostTags>(p =>
            {
                p.ToTable("PostTags");
                p.HasKey(p => p.Id);
                p.Property(e => e.Id).UseIdentityColumn();
                p.HasOne(e => e.Post)
                        .WithMany(e => e.PostTags)
                        .HasForeignKey(e => e.PostId)
                        .HasConstraintName("FK_PostTags_Post");

                p.HasOne(e => e.Tag)
                        .WithMany(e => e.PostTags)
                        .HasForeignKey(e => e.TagId)
                        .HasConstraintName("FK_PostTags_Tag");
            });
            // PostMetas
            modelBuilder.Entity<PostMeta>(p =>
            {
                p.ToTable("PostMetas");
                p.HasKey(p => p.Id);
                p.Property(e => e.Id).UseIdentityColumn();
                p.Property(x => x.Key).HasMaxLength(50);
                p.HasOne(e => e.Post)
                        .WithMany(e => e.PostMetas)
                        .HasForeignKey(e => e.PostId)
                        .HasConstraintName("FK_PostMetas_Post");
            });
            // Comment
            modelBuilder.Entity<Comment>(c =>
            {
                c.ToTable("Comments");
                c.HasKey(x => x.Id);
                c.Property(e => e.Id).UseIdentityColumn();
                c.Property(x => x.PostedBy);
                c.HasOne(x => x.ParentComment)
                    .WithMany(x => x.ChildComments)
                    .HasForeignKey(x => x.ParentCommentId)
                    .HasConstraintName("FK_CommentParent_CommentChild")
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

                c.HasOne(e => e.ParentPost)
                    .WithMany(e => e.Comments)
                    .HasForeignKey(e => e.PostId)
                    .HasConstraintName("FK_Comment_Post");
            });
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}