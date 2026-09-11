using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Represents a response to a remote command request.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record RemoteCommandResponse
{
    /// <summary>
    /// Gets the refusal reason, or <c>null</c> if the request was not refused.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RemoteCommandRefusal? Refusal { get; init; }

    /// <summary>
    /// Gets the command response data, or <c>null</c> if no data was returned.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public JsonElement? Data { get; init; }

    /// <summary>
    /// Creates a refused response.
    /// </summary>
    ///
    /// <param name="reason">The refusal reason.</param>
    ///
    /// <returns>A response containing the refusal reason.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static RemoteCommandResponse Refused(RemoteCommandRefusal reason) => new() { Refusal = reason };
}
