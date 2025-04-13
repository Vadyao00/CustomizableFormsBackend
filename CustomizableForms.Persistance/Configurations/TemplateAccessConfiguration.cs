using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableForms.Persistance.Configurations;

public class TemplateAccessConfiguration : IEntityTypeConfiguration<TemplateAccess>
{
    public void Configure(EntityTypeBuilder<TemplateAccess> builder)
    {
        builder.HasKey(ta => new { ta.TemplateId, ta.UserId });
        
        builder.HasOne(ta => ta.Template)
            .WithMany(t => t.AllowedUsers)
            .HasForeignKey(ta => ta.TemplateId);
            
        builder.HasOne(ta => ta.User)
            .WithMany(u => u.AccessibleTemplates)
            .HasForeignKey(ta => ta.UserId);
    }
}