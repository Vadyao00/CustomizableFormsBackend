using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableForms.Persistance.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd();
            
        builder.HasOne(a => a.Form)
            .WithMany(f => f.Answers)
            .HasForeignKey(a => a.FormId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}