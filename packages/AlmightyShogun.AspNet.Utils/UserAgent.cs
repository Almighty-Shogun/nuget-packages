using UAParser;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Represents simplified browser and device information parsed from a User-Agent header.
/// </summary>
///
/// <param name="Browser">The parsed browser family and major version.</param>
/// <param name="Device">The parsed operating-system family and major version.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.1</since>
public record UserAgent(string Browser, string Device)
{
    /// <summary>
    /// Parses a raw User-Agent header value into a simplified browser and device record.
    /// </summary>
    ///
    /// <param name="userAgent">The raw User-Agent header value to parse.</param>
    ///
    /// <returns>A <see cref="UserAgent"/> value containing browser and device information.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static UserAgent Parse(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return new UserAgent("Unknown", "Unknown");

        var parser = Parser.GetDefault();
        ClientInfo? client = parser.Parse(userAgent);

        string device = $"{client.OS.Family} {client.OS.Major}";
        string browser = $"{client.UA.Family} {client.UA.Major}";

        return new UserAgent(browser.Trim(), device.Trim());
    }
}
