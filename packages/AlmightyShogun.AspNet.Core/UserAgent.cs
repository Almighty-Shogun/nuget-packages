using UAParser;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// The parts of a User-Agent header worth recording: browser, operating system, device, and whether the caller is a
/// crawler. Every field is best-effort pattern matching on a header the client chooses, so none of it is trustworthy
/// enough to make an authorization or billing decision on.
/// </summary>
///
/// <param name="Browser">
/// The browser family with its major version, such as <c>Chrome 120</c>. The version is dropped when the header does
/// not carry one, leaving the family alone.
/// </param>
/// <param name="Os">
/// The operating-system family with its major version, such as <c>iOS 17</c>, formatted like <paramref name="Browser"/>.
/// </param>
/// <param name="Device">
/// The device family, such as <c>iPhone</c>. Desktop browsers report <c>Other</c> rather than a name, since a desktop
/// User-Agent does not identify the machine.
/// </param>
/// <param name="IsBot">
/// Whether the header matched a known crawler or spider. A bot that does not announce itself is reported as
/// <c>false</c>, so this filters honest traffic rather than defending against dishonest traffic.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.1</since>
public sealed record UserAgent(string Browser, string Os, string Device, bool IsBot)
{
    /// <summary>
    /// The placeholder every field takes when there is no header at all, distinct from the parser's own <c>Other</c>,
    /// which means a header was present but matched nothing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private const string Unknown = "Unknown";

    /// <summary>
    /// The shared parser instance. Creating one compiles the regular expression set, which is expensive enough that it
    /// must not happen per request.
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
    /// The parsed value, never <c>null</c>. An empty header yields <c>Unknown</c> throughout; an unrecognized one
    /// yields <c>Other</c> for whichever part failed to match.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static UserAgent Parse(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return new UserAgent(Unknown, Unknown, Unknown, false);

        ClientInfo client = Parser.Parse(userAgent);

        return new UserAgent(
            $"{client.UA.Family} {client.UA.Major}".Trim(),
            $"{client.OS.Family} {client.OS.Major}".Trim(),
            client.Device.Family.Trim(),
            client.Device.IsSpider
        );
    }
}
