using UAParser;

namespace AlmightyShogun.AspNet.Utils;

public static class UserAgentParser
{
    /// <summary>
    /// Parses a raw User-Agent string and returns a <see cref="UserAgent"/> value containing the browser and device information.
    /// </summary>
    ///
    /// <param name="userAgent">The raw User-Agent header value. If <c>null</c> or empty, the method returns a UserAgent with "Unknown" values.</param>
    ///
    /// <returns>A <see cref="UserAgent"/> instance with browser and device information.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    public static UserAgent Parse(string? userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return new UserAgent("Unknown", "Unknown");

        Parser? parser = Parser.GetDefault();
        ClientInfo? client = parser.Parse(userAgent);

        var device = $"{client.OS.Family} {client.OS.Major}";
        var browser = $"{client.UA.Family} {client.UA.Major}";

        return new UserAgent(browser.Trim(), device.Trim());
    }
}
