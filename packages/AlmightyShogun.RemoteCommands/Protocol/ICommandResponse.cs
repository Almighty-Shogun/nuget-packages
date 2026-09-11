namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Writes a command response.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface ICommandResponse
{
    /// <summary>
    /// Writes a response for the current command.
    /// </summary>
    ///
    /// <remarks>
    /// A command may write at most one response. If no response is written, the dispatcher sends an empty acknowledgement.
    /// </remarks>
    /// 
    /// <typeparam name="TResponse">
    /// The response type.
    /// </typeparam>
    /// <param name="data">
    /// The response value.
    /// </param>
    /// <param name="cancellationToken">A token used to cancel the write.</param>
    ///
    /// <returns>A task that completes when the response has been written.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// A response was already written for the current command.
    /// </exception>
    /// <exception cref="System.Text.Json.JsonException">
    /// <paramref name="data"/> could not be serialized.
    /// </exception>
    /// <exception cref="IOException">The response could not be written to the connection.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task WriteAsync<TResponse>(TResponse data, CancellationToken cancellationToken = default);
}
