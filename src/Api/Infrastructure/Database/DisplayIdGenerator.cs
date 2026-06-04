using NanoidDotNet;

namespace SaaS.Api.Infrastructure.Database;

public static class DisplayIdGenerator
{
    private const int RandomLength = 8;

    public static string NewTenantDisplayId() =>
        $"ten_{Nanoid.Generate(Nanoid.Alphabets.Default, RandomLength)}";

    public static string NewUserDisplayId() =>
        $"usr_{Nanoid.Generate(Nanoid.Alphabets.Default, RandomLength)}";
}
