using System.Text.Json;
using System.Reflection;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Provides the base implementation for a remote command that handles messages of type <typeparamref name="T"/>.
/// </summary>
///
/// <typeparam name="T">
/// The command message type.
/// </typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public abstract class RemoteCommand<T> : IRemoteCommand<T>, IInternalRemoteCommand where T : class
{
    /// <summary>
    /// The declared command name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly string _name;

    /// <summary>
    /// Initializes the remote command.
    /// </summary>
    ///
    /// <exception cref="InvalidOperationException">
    ///  The command type is not marked with <see cref="RemoteCommandAttribute"/>.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    protected RemoteCommand()
    {
        var attribute = GetType().GetCustomAttribute<RemoteCommandAttribute>();

        if (attribute is null)
            throw new InvalidOperationException($"Command {GetType().Name} must have {nameof(RemoteCommandAttribute)}.");

        _name = attribute.Name;
    }

    /// <summary>
    /// The declared command name, for a subclass that wants to name itself in its own logging or responses.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    protected string CommandName => _name;

    /// <inheritdoc />
    string IRemoteCommand.Name => _name;

    /// <inheritdoc />
    public abstract Task HandleCommandAsync(T message, ICommandResponse response, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    object IInternalRemoteCommand.Bind(JsonElement data)
    {
        if (data.ValueKind is JsonValueKind.Undefined)
            throw new JsonException($"The '{_name}' request did not contain data.");

        T? message = data.Deserialize<T>(RemoteCommandProtocol.SerializerOptions);

        if (message is null)
            throw new JsonException($"The '{_name}' payload could not be deserialized to {typeof(T).Name}.");

        return message;
    }

    /// <inheritdoc />
    Task IInternalRemoteCommand.ExecuteAsync(object message, ICommandResponse response, CancellationToken cancellationToken) =>
        HandleCommandAsync((T)message, response, cancellationToken);
}
