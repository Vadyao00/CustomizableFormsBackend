using CustomizableForms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableForms.Persistance.Configurations;

public class UserSalesforceProfileConfiguration : IEntityTypeConfiguration<UserSalesforceProfile>
{
    public void Configure(EntityTypeBuilder<UserSalesforceProfile> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(p => p.UserId)
            .IsRequired();
            
        builder.Property(p => p.SalesforceAccountId)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(p => p.SalesforceContactId)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(p => p.CreatedAt)
            .IsRequired();
            
        builder.Property(p => p.UpdatedAt)
            .IsRequired(false);
            
        builder.HasIndex(p => p.UserId)
            .IsUnique();
    }
}