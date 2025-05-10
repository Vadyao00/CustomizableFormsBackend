using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableForms.Persistance.Configurations;

public class TemplateCommentConfiguration : IEntityTypeConfiguration<TemplateComment>
{
    public void Configure(EntityTypeBuilder<TemplateComment> builder)
    {
        builder.HasKey(tc => tc.Id);
        
        builder.Property(tc => tc.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(tc => tc.Content)
            .IsRequired();
            
        builder.Property(tc => tc.CreatedAt)
            .IsRequired();
            
        builder.HasOne(tc => tc.Template)
            .WithMany(t => t.Comments)
            .HasForeignKey(tc => tc.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(tc => tc.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(tc => tc.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}