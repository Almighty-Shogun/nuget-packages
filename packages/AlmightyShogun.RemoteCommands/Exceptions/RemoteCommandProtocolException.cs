using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Thrown when a frame arrived and deserialized to <c>null</c>, meaning the server sent the literal <c>null</c> where an
/// envelope belongs. A frame that is malformed in any other way fails to deserialize and raises
/// <see cref="JsonException"/> instead, so this type does not cover every wire-format disagreement.
/// </summary>
///
/// <param name="message">What was wrong with the frame.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class RemoteCommandProtocolException(string message) : RemoteCommandException(message);
