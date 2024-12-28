namespace Jolt9;

public class OperatingSystemInfo
{
    private readonly Dictionary<string, string> props = new(StringComparer.OrdinalIgnoreCase);

    public string Id { get; set; } = string.Empty;

    public string? IdLike { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public string? VersionCodename { get; set; }

    public string? VersionId { get; set; }

    public string? PrettyName { get; set; }

    public string? AnsiColor { get; set; }

    public string? CpeName { get; set; }

    public string? HomeUrl { get; set; }

    public string? DocumentationUrl { get; set; }

    public string? SupportUrl { get; set; }

    public string? BugReportUrl { get; set; }

    public string? PrivacyPolicyUrl { get; set; }

    public string? BuildId { get; set; }

    public string? Variant { get; set; }

    public string? VariantId { get; set; }

    public string? this[string key]
    {
        get => this.props.TryGetValue(key, out var value) ? value : null;
        set
        {
            if (value is null)
            {
                this.props.Remove(key);
            }
            else
            {
                this.props[key] = value;
            }
        }
    }
}