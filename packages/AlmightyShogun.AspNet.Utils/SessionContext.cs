namespace AlmightyShogun.AspNet.Utils;

public record SessionContext(string? IpAddress, string? UserAgent)
{
    public const string ItemKey = nameof(SessionContext);
}
