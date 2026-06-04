using System.Text.RegularExpressions;

namespace SaaS.Api.Infrastructure.Database;

public static partial class SlugGenerator
{
    public static string FromName(string name)
    {
        string trimmed = name.Trim().ToLowerInvariant();
        string slug = NonAlphanumericRegex().Replace(trimmed, "-");
        slug = DuplicateHyphenRegex().Replace(slug, "-").Trim('-');
        return string.IsNullOrEmpty(slug) ? "workspace" : slug;
    }

    [GeneratedRegex(@"[^a-z0-9]+", RegexOptions.Compiled)]
    private static partial Regex NonAlphanumericRegex();

    [GeneratedRegex(@"-{2,}", RegexOptions.Compiled)]
    private static partial Regex DuplicateHyphenRegex();
}
