using System.Net;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Turns the textual <see cref="RemoteServerSettings"/> values into the network types the listener binds and matches
/// against, so the settings record stays a description of the configuration rather than a participant in reading it.
/// </summary>
///
/// <remarks>
/// Parsing lives here rather than on the record because a bound options type is reloaded, compared, and serialized, and a
/// method that throws on a value the binder happily accepted does not belong on something treated as data. Both members
/// are called once when the handler is constructed, so a bad value is reported when the listener is resolved.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class RemoteServerSettingsParser
{
    /// <summary>
    /// Parses the configured bind address, so an unusable value is reported by name rather than as a socket failure that
    /// says nothing about which setting produced it.
    /// </summary>
    ///
    /// <param name="value">The configured <c>RemoteServer:Address</c> value.</param>
    ///
    /// <returns>The address to bind.</returns>
    ///
    /// <exception cref="InvalidOperationException">The configured value is not an IP address.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static IPAddress ParseAddress(string value) => IPAddress.TryParse(value, out IPAddress? address)
        ? address
        : throw new InvalidOperationException($"RemoteServer:Address '{value}' is not an IP address.");

    /// <summary>
    /// Parses the configured whitelist into the networks a connecting address is matched against, accepting a CIDR range
    /// and a bare address through the same path.
    /// </summary>
    ///
    /// <param name="values">The configured <c>RemoteServer:Whitelisted</c> entries, each an address or a CIDR range.</param>
    ///
    /// <returns>
    /// One network per entry, a bare address becoming a single-address network so matching never has to special-case it.
    /// Empty when none are configured, which is what makes an unconfigured whitelist refuse every connection.
    /// </returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// An entry is neither an address nor a CIDR range. Rejected rather than skipped, because an entry that silently never
    /// matches leaves a whitelist that looks correct refusing the client it was written for.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static IReadOnlyList<IPNetwork> ParseWhitelist(IReadOnlyList<string> values)
    {
        List<IPNetwork> networks = [];

        foreach (string entry in values)
            if (IPNetwork.TryParse(entry, out IPNetwork network))
                networks.Add(network);
            else if (IPAddress.TryParse(entry, out IPAddress? address))
                networks.Add(new IPNetwork(address, address.GetAddressBytes().Length * 8));
            else
                throw new InvalidOperationException($"RemoteServer:Whitelisted entry '{entry}' is neither an IP address nor a CIDR range.");

        return networks;
    }
}
