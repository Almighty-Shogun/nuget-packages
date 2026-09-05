using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// The frame the listener sends back for every request, carrying either a refusal or the command's own response. Both
/// travel in one envelope so a client never has to guess which it received by inspecting the shape.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record RemoteCommandResponse
{
    /// <summary>
    /// Gets why the request was declined, or <c>null</c> when it was served. Serialized as the underlying number, so a
    /// value a newer server introduces still arrives and is kept here as that raw number. The client maps an unrecognized
    /// one onto <see cref="RemoteCommandRefusal.Other"/> when it builds the exception, not on this property.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RemoteCommandRefusal? Refusal { get; init; }

    /// <summary>
    /// Gets whatever the command wrote, or <c>null</c> when it was refused or ran without writing anything. A command
    /// that returns without answering is therefore acknowledged rather than leaving the client waiting.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public JsonElement? Data { get; init; }

    /// <summary>
    /// Builds the envelope for a refusal.
    /// </summary>
    ///
    /// <param name="reason">What the listener objected to.</param>
    ///
    /// <returns>The envelope to send, carrying the reason and no data.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static RemoteCommandResponse Refused(RemoteCommandRefusal reason) => new() { Refusal = reason };
}
