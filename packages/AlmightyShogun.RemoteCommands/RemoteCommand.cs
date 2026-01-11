using System.Text;
using System.Text.Json;
using System.Reflection;
using System.Net.Sockets;

namespace AlmightyShogun.RemoteCommands;

public abstract class RemoteCommand<T> : IRemoteCommand<T> where T : class
{
    public virtual string Name
    {
        get
        {
            var attribute = GetType().GetCustomAttribute<RemoteCommandAttribute>();

            return attribute == null ? throw new InvalidOperationException($"Command {GetType().Name} must have RemoteCommandAttribute") : attribute.Name;
        }
    }

    /// <inheritdoc />
    public abstract Task HandleCommandAsync(T message, NetworkStream stream);

    /// <inheritdoc />
    public async Task HandleRawAsync(JsonElement data, NetworkStream stream)
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
