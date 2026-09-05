namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Why the server declined to run a command. This is the vocabulary both ends share and the only place a refusal is
/// named.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public enum RemoteCommandRefusal
{
    /// <summary>
    /// The request was accepted and handed to its command, but serving it failed for a reason with no more specific code
    /// here. That covers a command that ran and threw as well as a request the command could never be given, so this on
    /// its own does not say whether the command's own body ran.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Other = 0,

    /// <summary>
    /// The request was not readable as JSON, so the server never looked for a command name. This package's own client
    /// serializes every request before sending it, so from such a client the bytes were valid when they left and the
    /// fault is on the connection: frames out of step with each other, or corruption in transit.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    MalformedPayload,

    /// <summary>
    /// The request parsed but named no command. A blank or whitespace name produces this rather than
    /// <see cref="CommandNotFound"/>, because there was nothing to look up.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    MissingCommandName,

    /// <summary>
    /// The server requires a pre-shared key and the one sent did not match, or none was sent. The address was
    /// whitelisted, otherwise the connection would have been dropped without an answer.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Unauthorized,

    /// <summary>
    /// No command is registered under that name. Names match with ordinal case sensitivity, so this is as likely to be a
    /// difference in capitalization as a command the server does not have.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    CommandNotFound,

    /// <summary>
    /// The command exists but a <c>JsonException</c> escaped it, usually because the data could not become its message type,
    /// a property carried the wrong JSON type, or the payload was not an object. A command that itself raises one after
    /// running is reported the same way. An omitted property binds to its default, unless the message marks it
    /// <c>required</c>, which is refused here the same way a wrong type is.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    InvalidMessage
}
