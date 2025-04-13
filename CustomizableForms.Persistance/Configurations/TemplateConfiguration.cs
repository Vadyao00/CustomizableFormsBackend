using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableForms.Persistance.Configurations;

public class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(t => t.Topic)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(t => t.CreatedAt)
            .IsRequired();
            
        builder.HasOne(t => t.Creator)
            .WithMany(u => u.CreatedTemplates)
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}