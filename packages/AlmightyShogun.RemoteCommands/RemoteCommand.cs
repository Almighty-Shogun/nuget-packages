using System.Text;
using System.Text.Json;
using System.Reflection;
using System.Net.Sockets;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Base class for remote commands that handle a typed JSON payload.
/// </summary>
///
/// <typeparam name="T">The command message type deserialized from the incoming payload data.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public abstract class RemoteCommand<T> : IRemoteCommand<T>, IInternalRemoteCommand where T : class
{
    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    string IRemoteCommand.Name
    {
        get
        {
            var attribute = GetType().GetCustomAttribute<RemoteCommandAttribute>();

            return attribute == null ? throw new InvalidOperationException($"Command {GetType().Name} must have RemoteCommandAttribute") : attribute.Name;
        }
    }

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public abstract Task HandleCommandAsync(T message, NetworkStream stream);

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    async Task IInternalRemoteCommand.HandleRawAsync(JsonElement data, NetworkStream stream)
    {
        var message = data.Deserialize<T>();

        if (message is null) return;

        await HandleCommandAsync(message, stream);
    }

    /// <summary>
    /// Writes a serialized response object to the specified network stream and flushes the stream.
    /// </summary>
    ///
    /// <param name="stream">The network stream used to write the response.</param>
    /// <param name="data">The object to be serialized and sent over the network stream.</param>
    ///
    /// <returns>A task representing the asynchronous write and flush operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    protected async Task WriteResponseAsync(NetworkStream stream, object data)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));

        await stream.WriteAsync(buffer);
        await stream.FlushAsync();
    }
}
