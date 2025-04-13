using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableForms.Persistance.Configurations;

public class FormConfiguration : IEntityTypeConfiguration<Form>
{
    public void Configure(EntityTypeBuilder<Form> builder)
    {
        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(f => f.SubmittedAt)
            .IsRequired();
            
        builder.HasOne(f => f.Template)
            .WithMany(t => t.Forms)
            .HasForeignKey(f => f.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(f => f.User)
            .WithMany(u => u.SubmittedForms)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}