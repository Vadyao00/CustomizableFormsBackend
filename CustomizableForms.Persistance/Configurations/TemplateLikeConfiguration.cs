using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableForms.Persistance.Configurations;

public class TemplateLikeConfiguration : IEntityTypeConfiguration<TemplateLike>
{
    public void Configure(EntityTypeBuilder<TemplateLike> builder)
    {
        builder.HasKey(tl => tl.Id);
        
        builder.Property(tl => tl.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(tl => tl.CreatedAt)
            .IsRequired();
            
        builder.HasOne(tl => tl.Template)
            .WithMany(t => t.Likes)
            .HasForeignKey(tl => tl.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(tl => tl.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(tl => tl.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(tl => new { tl.TemplateId, tl.UserId })
            .IsUnique();
    }
}