using CustomizableForms.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableForms.Persistance.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(r => r.NormalizedName)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasIndex(r => r.NormalizedName)
            .IsUnique();
            
        builder.HasData(
            new Role
            {
                Id = Guid.Parse("8D04DCE2-969A-435D-BBA4-DF3F325983DC"),
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new Role
            {
                Id = Guid.Parse("2C5E174E-3B0E-446F-86AF-483D56FD7210"),
                Name = "User",
                NormalizedName = "USER"
            }
        );
    }
}