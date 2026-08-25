using System.Text.Json;
using System.Reflection;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// The base every remote command inherits. It reads the command name from the class attribute once and binds each
/// request's payload to <typeparamref name="T"/> before handing it to the subclass.
/// </summary>
///
/// <typeparam name="T">
/// The message this command expects. Its shape is part of the command's wire contract, so changing it changes what
/// existing clients must send.
/// </typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public abstract class RemoteCommand<T> : IRemoteCommand<T>, IInternalRemoteCommand where T : class
{
    /// <summary>
    /// The command name, read once from the attribute. Reading it per access would reflect on every dispatch, and a
    /// missing attribute would not surface until the first request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly string _name;

    /// <summary>
    /// Reads and caches the declared command name. This runs when the listener resolves the command rather than on the
    /// first request, so a class missing its attribute is reported at startup instead of to whoever calls it first.
    /// </summary>
    ///
    /// <exception cref="InvalidOperationException">
    /// The class does not carry <see cref="RemoteCommandAttribute"/>, so it declares no name to be reachable by.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected RemoteCommand()
    {
        var attribute = GetType().GetCustomAttribute<RemoteCommandAttribute>();

        if (attribute is null)
            throw new InvalidOperationException($"Command {GetType().Name} must have {nameof(RemoteCommandAttribute)}.");

        _name = attribute.Name;
    }

    /// <summary>
    /// Gets the declared command name, for a subclass that wants to name itself in its own logging or responses.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected string CommandName => _name;

    /// <inheritdoc />
    string IRemoteCommand.Name => _name;

    /// <inheritdoc />
    public abstract Task HandleCommandAsync(T message, ICommandResponse response, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    async Task IInternalRemoteCommand.HandleRawAsync(JsonElement data, ICommandResponse response, CancellationToken cancellationToken)
    {
        var message = data.Deserialize<T>(RemoteCommandProtocol.SerializerOptions);

        if (message is null)
            throw new JsonException($"The '{_name}' payload did not contain the properties {typeof(T).Name} requires.");

        await HandleCommandAsync(message, response, cancellationToken);
    }
}
