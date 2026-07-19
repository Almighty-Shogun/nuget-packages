using System.Text.Json;

namespace AlmightyShogun.Utils;

/// <summary>
/// Provides JSON serialization and deserialization helpers for values, strings, and streams.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class DeserializeExtensions
{
    /// <summary>
    /// Stores the package default JSON options used by overloads that request default options.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Provides JSON deserialization extension methods for string payloads.
    /// </summary>
    ///
    /// <param name="json">The JSON string payload to deserialize.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    extension(string json)
    {
        /// <summary>
        /// Deserializes the specified JSON string to an object of type <typeparamref name="T"/>.
        /// </summary>
        ///
        /// <param name="options">The JSON serializer options to apply.</param>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        ///
        /// <returns>The deserialized value, or <c>null</c> when the JSON payload resolves to null.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.1.0</since>
        public T? Deserialize<T>(JsonSerializerOptions options)
            => JsonSerializer.Deserialize<T>(json, options);

        /// <summary>
        /// Deserializes the specified JSON string to an object of type <typeparamref name="T"/>.
        /// </summary>
        ///
        /// <param name="useDefaultOptions">Whether to use the package default options with camel-case property names.</param>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        ///
        /// <returns>The deserialized value, or <c>null</c> when the JSON payload resolves to null.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.1.0</since>
        public T? Deserialize<T>(bool useDefaultOptions = true)
            => JsonSerializer.Deserialize<T>(json, useDefaultOptions ? _jsonSerializerOptions : null);
    }

    /// <summary>
    /// Provides JSON deserialization extension methods for readable streams.
    /// </summary>
    ///
    /// <param name="stream">The stream containing the JSON payload to deserialize.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    extension(Stream stream)
    {
        /// <summary>
        /// Asynchronously deserializes the specified JSON stream to an object of type <typeparamref name="T"/>.
        /// </summary>
        ///
        /// <param name="options">The JSON serializer options to apply.</param>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        ///
        /// <returns>A task containing the deserialized value, or <c>null</c> when the JSON payload resolves to null.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.1.0</since>
        public async Task<T?> DeserializeAsync<T>(JsonSerializerOptions options)
            => await JsonSerializer.DeserializeAsync<T>(stream, options);

        /// <summary>
        /// Asynchronously deserializes the specified JSON stream to an object of type <typeparamref name="T"/>.
        /// </summary>
        ///
        /// <param name="useDefaultOptions">Whether to use the package default options with camel-case property names.</param>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        ///
        /// <returns>A task containing the deserialized value, or <c>null</c> when the JSON payload resolves to null.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.1.0</since>
        public async Task<T?> DeserializeAsync<T>(bool useDefaultOptions = true)
            => await JsonSerializer.DeserializeAsync<T>(stream, useDefaultOptions ? _jsonSerializerOptions : null);
    }

    /// <summary>
    /// Provides JSON serialization extension methods for the target value.
    /// </summary>
    ///
    /// <param name="value">The value to serialize.</param>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension<T>(T value)
    {
        /// <summary>
        /// Serializes the specified value to a JSON string.
        /// </summary>
        ///
        /// <param name="options">The JSON serializer options to apply.</param>
        ///
        /// <returns>The JSON string produced by <see cref="JsonSerializer"/>.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public string Serialize(JsonSerializerOptions options)
            => JsonSerializer.Serialize(value, options);

        /// <summary>
        /// Serializes the specified value to a JSON string.
        /// </summary>
        ///
        /// <param name="useDefaultOptions">Whether to use the package default options with camel-case property names.</param>
        ///
        /// <returns>The JSON string produced by <see cref="JsonSerializer"/>.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public string Serialize(bool useDefaultOptions = true)
            => JsonSerializer.Serialize(value, useDefaultOptions ? _jsonSerializerOptions : null);

        /// <summary>
        /// Asynchronously serializes the specified value to a JSON stream.
        /// </summary>
        ///
        /// <param name="stream">The writable stream that receives the JSON payload.</param>
        /// <param name="options">The JSON serializer options to apply.</param>
        ///
        /// <returns>A task that completes when the value has been serialized to the stream.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public async Task SerializeAsync(Stream stream, JsonSerializerOptions options)
            => await JsonSerializer.SerializeAsync(stream, value, options);

        /// <summary>
        /// Asynchronously serializes the specified value to a JSON stream.
        /// </summary>
        ///
        /// <param name="stream">The writable stream that receives the JSON payload.</param>
        /// <param name="useDefaultOptions">Whether to use the package default options with camel-case property names.</param>
        ///
        /// <returns>A task that completes when the value has been serialized to the stream.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public async Task SerializeAsync(Stream stream, bool useDefaultOptions = true)
            => await JsonSerializer.SerializeAsync(stream, value, useDefaultOptions ? _jsonSerializerOptions : null);
    }
}
