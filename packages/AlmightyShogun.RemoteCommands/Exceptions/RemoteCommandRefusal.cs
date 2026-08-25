namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Why the server declined to run a command. Sent on the wire as its underlying number, so this is the vocabulary both
/// ends share and the only place a refusal is named.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public enum RemoteCommandRefusal
{
    /// <summary>
    /// A reason this client has no name for, which is how a value introduced by a newer server arrives. The value itself
    /// is not kept, so all that survives is that the server refused and this client could not say why.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Other = 0,

    /// <summary>
    /// The request was not readable as JSON, so the server never looked for a command name. From this package's own
    /// client that means the frame was corrupted in transit.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    MalformedPayload,

    /// <summary>
    /// The request parsed but named no command. A blank or whitespace name produces this rather than
    /// <see cref="CommandNotFound"/>, because there was nothing to look up.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    MissingCommandName,

    /// <summary>
    /// The server requires a pre-shared key and the one sent did not match, or none was sent. The address was
    /// whitelisted, otherwise the connection would have been dropped without an answer.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Unauthorized,

    /// <summary>
    /// No command is registered under that name. Names match with ordinal case sensitivity, so this is as likely to be a
    /// difference in capitalization as a command the server does not have.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    CommandNotFound,

    /// <summary>
    /// The command exists but the data could not become its message type, because a property carried the wrong JSON type
    /// or the payload was not an object. A property the payload simply omits binds to the default instead.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    InvalidMessage
}
