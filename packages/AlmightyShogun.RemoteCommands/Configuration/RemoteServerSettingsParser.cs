using System.Net;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Parses remote server network settings.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal static class RemoteServerSettingsParser
{
    /// <summary>
    /// Parses the configured bind address.
    /// </summary>
    ///
    /// <param name="value">The configured address.</param>
    ///
    /// <returns>The parsed IP address.</returns>
    ///
    /// <exception cref="InvalidOperationException">The configured value is not a valid IP address.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static IPAddress ParseAddress(string value) => IPAddress.TryParse(value, out IPAddress? address)
        ? address
        : throw new InvalidOperationException($"RemoteServer:Address '{value}' is not an IP address.");

    /// <summary>
    /// Parses the configured whitelist entries as IP networks.
    /// </summary>
    ///
    /// <param name="values">The configured IP addresses or CIDR ranges.</param>
    ///
    /// <returns>
    /// The parsed IP networks.
    /// </returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// A configured entry is neither a valid IP address nor a CIDR range.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
