using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableForms.Persistance.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasKey(q => q.Id);
        
        builder.Property(q => q.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(q => q.Title)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(q => q.OrderIndex)
            .IsRequired();
            
        builder.Property(q => q.Type)
            .IsRequired();
            
        builder.HasOne(q => q.Template)
            .WithMany(t => t.Questions)
            .HasForeignKey(q => q.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}