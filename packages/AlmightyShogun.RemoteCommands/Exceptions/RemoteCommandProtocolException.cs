using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Thrown when the server returns an invalid protocol response.
/// </summary>
///
/// <param name="message">The error message.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class RemoteCommandProtocolException(string message) : RemoteCommandException(message);
