using UAParser;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Represents parsed information from a User-Agent header.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.1</since>
public sealed record UserAgent
{
    /// <summary>
    /// The browser family and major version.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.1</since>
    public required string Browser { get; init; }

    /// <summary>
    /// The operating system family and major version.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string Os { get; init; }

    /// <summary>
    /// The device family.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.1</since>
    public required string Device { get; init; }

    /// <summary>
    /// Whether the User-Agent identifies a known crawler or spider.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required bool IsBot { get; init; }
    
    private const string _unknown = "Unknown";
    
    private static readonly Parser _parser = Parser.GetDefault();

    /// <summary>
    /// Parses a User-Agent header.
    /// </summary>
    ///
    /// <param name="userAgent">The User-Agent header value.</param>
    ///
    /// <returns>The parsed information, or unknown values when the header is empty.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public static UserAgent Parse(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return new UserAgent
            {
                Browser = _unknown,
                Os = _unknown,
                Device = _unknown,
                IsBot = false
            };

        ClientInfo client = _parser.Parse(userAgent);

        return new UserAgent
        {
            Browser = $"{client.UA.Family} {client.UA.Major}".Trim(),
            Os = $"{client.OS.Family} {client.OS.Major}".Trim(),
            Device = client.Device.Family.Trim(),
            IsBot = client.Device.IsSpider
        };
    }
}
