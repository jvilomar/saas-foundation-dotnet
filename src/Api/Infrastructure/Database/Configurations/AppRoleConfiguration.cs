using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SaaS.Api.Infrastructure.Database.Entities;

namespace SaaS.Api.Infrastructure.Database.Configurations;

public sealed class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(256);

        builder.Property(r => r.IsSystem)
            .IsRequired();

        builder.HasIndex(r => r.Name)
            .IsUnique();
    }
}
