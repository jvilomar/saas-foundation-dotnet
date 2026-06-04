using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SaaS.Api.Infrastructure.Database.Entities;
using SaaS.Api.Infrastructure.Database.ValueGenerators;

namespace SaaS.Api.Infrastructure.Database.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.DisplayId)
            .HasMaxLength(16)
            .IsRequired()
            .HasValueGenerator<TenantDisplayIdValueGenerator>();

        builder.Property(t => t.Slug)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(t => t.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.HasIndex(t => t.Slug)
            .IsUnique();

        builder.HasIndex(t => t.DisplayId)
            .IsUnique();

        builder.HasIndex(t => t.Name)
            .IsUnique();
    }
}
