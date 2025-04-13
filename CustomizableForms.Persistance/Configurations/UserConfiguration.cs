using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableForms.Persistance.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(256);
            
        builder.Property(e => e.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);
            
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.RefreshToken)
            .HasMaxLength(512);
            
        builder.Property(e => e.PreferredLanguage)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("en");

        builder.Property(e => e.PreferredTheme)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("light");

        builder.HasIndex(e => e.Name)
            .IsUnique();
            
        builder.HasIndex(e => e.Email)
            .IsUnique();
    }
}