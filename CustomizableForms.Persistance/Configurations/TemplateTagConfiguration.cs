using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableForms.Persistance.Configurations;

public class TemplateTagConfiguration : IEntityTypeConfiguration<TemplateTag>
{
    public void Configure(EntityTypeBuilder<TemplateTag> builder)
    {
        builder.HasKey(tt => new { tt.TemplateId, tt.TagId });
        
        builder.HasOne(tt => tt.Template)
            .WithMany(t => t.TemplateTags)
            .HasForeignKey(tt => tt.TemplateId);
            
        builder.HasOne(tt => tt.Tag)
            .WithMany(t => t.TemplateTags)
            .HasForeignKey(tt => tt.TagId);
    }
}