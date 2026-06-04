using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace SaaS.Api.Infrastructure.Database.ValueGenerators;

public sealed class UserDisplayIdValueGenerator : ValueGenerator<string>
{
    public override bool GeneratesTemporaryValues => false;

    public override string Next(EntityEntry entry) => DisplayIdGenerator.NewUserDisplayId();
}
