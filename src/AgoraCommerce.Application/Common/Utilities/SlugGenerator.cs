using System.Text;
using System.Text.RegularExpressions;

namespace AgoraCommerce.Application.Common.Utilities;

public static partial class SlugGenerator
{
    public static string Generate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim().ToLowerInvariant();
        normalized = MultipleWhitespaceRegex().Replace(normalized, " ");

        var sb = new StringBuilder();
        foreach (var ch in normalized)
        {
            if (char.IsLetterOrDigit(ch))
            {
                sb.Append(ch);
            }
            else if (ch == ' ' || ch == '-' || ch == '_')
            {
                sb.Append('-');
            }
        }

        var slug = MultipleHyphenRegex().Replace(sb.ToString(), "-").Trim('-');
        return slug;
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex MultipleWhitespaceRegex();

    [GeneratedRegex(@"-+")]
    private static partial Regex MultipleHyphenRegex();
}
