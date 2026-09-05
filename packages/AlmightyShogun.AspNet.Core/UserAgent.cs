using UAParser;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// The parts of a User-Agent header worth recording: browser, operating system, device, and whether the caller is a
/// crawler. Every field is best-effort pattern matching on a header the client chooses, so none of it is trustworthy
/// enough to make an authorization or billing decision on.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.1</since>
public sealed record UserAgent
{
    /// <summary>
    /// Gets the browser family with its major version, such as <c>Chrome 120</c>. The version is dropped when the header
    /// does not carry one, leaving the family alone.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.1</since>
    public required string Browser { get; init; }

    /// <summary>
    /// Gets the operating-system family with its major version, such as <c>iOS 17</c>, formatted like
    /// <see cref="Browser"/>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.1</since>
    public required string Os { get; init; }

    /// <summary>
    /// Gets the device family, such as <c>iPhone</c>. Desktop browsers report <c>Other</c> rather than a name, since a
    /// desktop User-Agent does not identify the machine.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.1</since>
    public required string Device { get; init; }

    /// <summary>
    /// Gets whether the header matched a known crawler or spider. A bot that does not announce itself is reported as
    /// <c>false</c>, so this filters honest traffic rather than defending against dishonest traffic.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.1</since>
    public required bool IsBot { get; init; }

    /// <summary>
    /// The placeholder the three string fields take when there is no header at all, distinct from the parser's own <c>Other</c>,
    /// which means a header was present but matched nothing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private const string Unknown = "Unknown";

    /// <summary>
    /// The shared parser instance. Creating one re-reads the embedded pattern set and rebuilds every regular expression,
    /// which is expensive enough that it must not happen per request. The patterns are interpreted rather than compiled,
    /// since the parser is built without that option.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly Parser Parser = Parser.GetDefault();

    /// <summary>
    /// Turns a raw header into the four recorded parts, falling back to placeholders rather than failing on a header it
    /// cannot read.
    /// </summary>
    ///
    /// <param name="userAgent">
    /// The header value as sent. An empty or absent value short-circuits before the parser runs, so no regular
    /// expression is evaluated for a request that carried no header.
    /// </param>
    ///
    /// <returns>
    /// The parsed value, never <c>null</c>. An empty header yields <c>Unknown</c> for all three strings and <c>false</c> for
    /// <see cref="IsBot"/>; an unrecognized one
    /// yields <c>Other</c> for whichever part failed to match.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static UserAgent Parse(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return new UserAgent
            {
                Browser = Unknown,
                Os = Unknown,
                Device = Unknown,
                IsBot = false
            };

        ClientInfo client = Parser.Parse(userAgent);

        return new UserAgent
        {
            Browser = $"{client.UA.Family} {client.UA.Major}".Trim(),
            Os = $"{client.OS.Family} {client.OS.Major}".Trim(),
            Device = client.Device.Family.Trim(),
            IsBot = client.Device.IsSpider
        };
    }
}
