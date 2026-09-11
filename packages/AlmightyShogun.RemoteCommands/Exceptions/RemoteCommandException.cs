using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Represents an error reported by <see cref="RemoteCommandClient"/>.
/// </summary>
///
/// <param name="message">The error message.</param>
/// <param name="innerException">
/// The underlying exception, if available.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public abstract class RemoteCommandException(string message, Exception? innerException = null) : Exception(message, innerException);
