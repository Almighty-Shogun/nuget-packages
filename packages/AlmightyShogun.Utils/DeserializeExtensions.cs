using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace AlmightyShogun.Utils;

/// <summary>
/// Provides JSON deserialization for strings and streams.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class DeserializeExtensions
{
    /// <summary>
    /// Gets the default JSON serializer options.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static JsonSerializerOptions DefaultOptions { get; } = CreateDefaultOptions();

    /// <summary>
    /// Creates the default read-only JSON serializer options.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static JsonSerializerOptions CreateDefaultOptions()
    {
        JsonSerializerOptions options = new(JsonSerializerDefaults.Web);

        options.MakeReadOnly(true);

        return options;
    }

    /// <summary>
    /// Provides JSON deserialization for strings.
    /// </summary>
    ///
    /// <param name="json">
    /// The JSON to deserialize.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    extension(string json)
    {
        /// <summary>
        /// Attempts to deserialize the JSON into <typeparamref name="T"/>.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// The type to deserialize.
        /// </typeparam>
        /// <param name="result">
        /// When this method returns <c>true</c>, contains the deserialized value, otherwise the default value.
        /// </param>
        /// <param name="options">
        /// The serializer options to use. If <c>null</c>, the default options are used.
        /// </param>
        ///
        /// <returns><c>true</c> if a non-null value was deserialized; otherwise, <c>false</c>.</returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// The JSON string is <c>null</c>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// There is no compatible converter for <typeparamref name="T"/> or its members.
        /// </exception>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public bool TryDeserialize<T>([NotNullWhen(true)] out T? result, JsonSerializerOptions? options = null)
        {
            try
            {
                var value = JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);

                result = value!;

                return value is not null;
            }
            catch (JsonException)
            {
                result = default!;

                return false;
            }
        }
    }

    /// <summary>
    /// Provides JSON deserialization for streams.
    /// </summary>
    ///
    /// <param name="stream">
    /// The stream to deserialize.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    extension(Stream stream)
    {
        /// <summary>
        /// Asynchronously deserializes the stream into <typeparamref name="T"/>.
        /// </summary>
        ///
        /// <typeparam name="T">The type to deserialize.</typeparam>
        /// <param name="options">
        /// The serializer options to use. If <c>null</c>, the default options are used.
        /// </param>
        /// <param name="cancellationToken">
        /// A token used to cancel the operation.
        /// </param>
        ///
        /// <returns>
        /// The deserialized value, or <c>null</c> if the JSON represents <c>null</c>.
        /// </returns>
        ///
        /// <exception cref="JsonException">
        /// The JSON is invalid or cannot be deserialized into <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <typeparamref name="T"/> is not supported for deserialization.
        /// </exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.1.0</since>
        public async Task<T?> DeserializeAsync<T>(
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default
        ) => await JsonSerializer.DeserializeAsync<T>(stream, options ?? DefaultOptions, cancellationToken);
    }
}
