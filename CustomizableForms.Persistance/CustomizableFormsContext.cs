using CustomizableForms.Domain.Entities;
using CustomizableForms.Persistance.Configurations;
using CustomizableForms.Persistance.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CustomizableForms.Persistance;

public class CustomizableFormsContext : DbContext
{
    public CustomizableFormsContext()
    { }

    public CustomizableFormsContext(DbContextOptions<CustomizableFormsContext> options)
        : base(options)
    { }
    
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<Template> Templates { get; set; }
    public virtual DbSet<Question> Questions { get; set; }
    public virtual DbSet<Form> Forms { get; set; }
    public virtual DbSet<Answer> Answers { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }
    public virtual DbSet<TemplateTag> TemplateTags { get; set; }
    public virtual DbSet<TemplateAccess> TemplateAccesses { get; set; }
    public virtual DbSet<TemplateComment> TemplateComments { get; set; }
    public virtual DbSet<TemplateLike> TemplateLikes { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new TemplateConfiguration());
        modelBuilder.ApplyConfiguration(new QuestionConfiguration());
        modelBuilder.ApplyConfiguration(new FormConfiguration());
        modelBuilder.ApplyConfiguration(new AnswerConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());
        modelBuilder.ApplyConfiguration(new TemplateTagConfiguration());
        modelBuilder.ApplyConfiguration(new TemplateAccessConfiguration());
        modelBuilder.ApplyConfiguration(new TemplateCommentConfiguration());
        modelBuilder.ApplyConfiguration(new TemplateLikeConfiguration());

        modelBuilder.SeedInitialData();
    }
}