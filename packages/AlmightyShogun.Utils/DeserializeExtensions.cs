using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace AlmightyShogun.Utils;

/// <summary>
/// Provides JSON deserialization helpers for strings and streams. Each helper comes in two forms: one taking explicit
/// <see cref="JsonSerializerOptions"/>, and one selecting between the package defaults and the
/// <see cref="System.Text.Json"/> defaults with a flag, so the common case needs no options object at the call site.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class DeserializeExtensions
{
    /// <summary>
    /// Gets the package default JSON options, which apply camel-case property naming so a payload from an ASP.NET Core API
    /// binds without further configuration. The instance is read-only, so it can be shared across the process and passed to
    /// <see cref="JsonSerializer"/> directly without any caller being able to alter it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static JsonSerializerOptions DefaultOptions { get; } = CreateDefaultOptions();

    /// <summary>
    /// Creates the package default options and seals them, so the shared instance cannot be mutated after publication.
    /// </summary>
    ///
    /// <returns>The sealed options, on which any further attempt to set a property throws.</returns>
    ///
    /// <remarks>
    /// Sealing is what makes a single static instance safe. A shared mutable <see cref="JsonSerializerOptions"/> could be
    /// altered by any caller and would silently change deserialization for every other caller in the process.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static JsonSerializerOptions CreateDefaultOptions()
    {
        JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        options.MakeReadOnly(true);

        return options;
    }

    /// <summary>
    /// Provides JSON deserialization for a payload already held in memory.
    /// </summary>
    ///
    /// <param name="json">
    /// The JSON text to read. Deserialized in full, so the whole document must be present; use the stream overloads to avoid
    /// materializing a large payload as a string first.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    extension(string json)
    {
        /// <summary>
        /// Deserializes the JSON text into <typeparamref name="T"/> using the supplied options.
        /// </summary>
        ///
        /// <typeparam name="T">The type to bind the payload to.</typeparam>
        /// <param name="options">
        /// The serializer options to apply, in place of the package defaults. Use this overload when the payload needs a
        /// naming policy, converter, or tolerance the defaults do not provide.
        /// </param>
        ///
        /// <returns>The deserialized value, or <c>null</c> when the payload is the JSON literal <c>null</c>.</returns>
        ///
        /// <exception cref="JsonException">
        /// The text is not valid JSON, carries data after the first document, or cannot bind to <typeparamref name="T"/>.
        /// Use <see cref="TryDeserialize{T}(string, out T, JsonSerializerOptions)"/> when invalid input is expected.
        /// </exception>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.1.0</since>
        public T? Deserialize<T>(JsonSerializerOptions options) => JsonSerializer.Deserialize<T>(json, options);

        /// <summary>
        /// Deserializes the JSON text into <typeparamref name="T"/> using the package defaults.
        /// </summary>
        ///
        /// <typeparam name="T">The type to bind the payload to.</typeparam>
        /// <param name="useDefaultOptions">
        /// Whether to apply the package defaults, which use camel-case property names. Pass <c>false</c> to fall back to the
        /// <see cref="System.Text.Json"/> defaults, which match property names exactly.
        /// </param>
        ///
        /// <returns>The deserialized value, or <c>null</c> when the payload is the JSON literal <c>null</c>.</returns>
        ///
        /// <exception cref="JsonException">
        /// The text is not valid JSON, carries data after the first document, or cannot bind to <typeparamref name="T"/>.
        /// Use <see cref="TryDeserialize{T}(string, out T, bool)"/> when invalid input is expected.
        /// </exception>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.1.0</since>
        public T? Deserialize<T>(bool useDefaultOptions = true)
            => JsonSerializer.Deserialize<T>(json, useDefaultOptions ? DefaultOptions : null);

        /// <summary>
        /// Deserializes the JSON text into <typeparamref name="T"/> using the supplied options, without throwing on malformed
        /// input. Use it for a request body, queue message, or user-supplied file where invalid JSON is an expected outcome
        /// rather than a fault.
        /// </summary>
        ///
        /// <typeparam name="T">The type to bind the payload to.</typeparam>
        /// <param name="result">
        /// When this method returns <c>true</c>, contains the deserialized value, annotated so the compiler treats it as
        /// non-null from that point and no suppression is needed. Left at the default for <typeparamref name="T"/> otherwise.
        /// </param>
        /// <param name="options">The serializer options to apply, in place of the package defaults.</param>
        ///
        /// <returns><c>true</c> when a non-null value was read; otherwise <c>false</c>.</returns>
        ///
        /// <remarks>
        /// Only <see cref="JsonException"/> is caught. Any other failure still propagates, so a genuine programming error is
        /// not swallowed by a method whose name suggests it only reports success or failure.
        ///
        /// A payload that is the JSON literal <c>null</c> reports <c>false</c> rather than succeeding with a null value, so a
        /// <c>true</c> result always yields something usable. Call <see cref="Deserialize{T}(string, JsonSerializerOptions)"/>
        /// instead when that case must be told apart from malformed input.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public bool TryDeserialize<T>([NotNullWhen(true)] out T? result, JsonSerializerOptions options)
        {
            try
            {
                var value = JsonSerializer.Deserialize<T>(json, options);

                result = value!;

                return value is not null;
            }
            catch (JsonException)
            {
                result = default!;

                return false;
            }
        }

        /// <summary>
        /// Deserializes the JSON text into <typeparamref name="T"/> using the package defaults, without throwing on malformed
        /// input.
        /// </summary>
        ///
        /// <typeparam name="T">The type to bind the payload to.</typeparam>
        /// <param name="result">
        /// When this method returns <c>true</c>, contains the deserialized value, annotated so the compiler treats it as
        /// non-null from that point and no suppression is needed. Left at the default for <typeparamref name="T"/> otherwise.
        /// </param>
        /// <param name="useDefaultOptions">
        /// Whether to apply the package defaults, which use camel-case property names. Pass <c>false</c> to fall back to the
        /// <see cref="System.Text.Json"/> defaults, which match property names exactly.
        /// </param>
        ///
        /// <returns><c>true</c> when a non-null value was read; otherwise <c>false</c>.</returns>
        ///
        /// <remarks>
        /// Only <see cref="JsonException"/> is caught. Any other failure still propagates, so a genuine programming error is
        /// not swallowed by a method whose name suggests it only reports success or failure.
        ///
        /// A payload that is the JSON literal <c>null</c> reports <c>false</c> rather than succeeding with a null value, so a
        /// <c>true</c> result always yields something usable. Call <see cref="Deserialize{T}(string, bool)"/> instead when
        /// that case must be told apart from malformed input.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public bool TryDeserialize<T>([NotNullWhen(true)] out T? result, bool useDefaultOptions = true)
            => json.TryDeserialize(out result, useDefaultOptions ? DefaultOptions : JsonSerializerOptions.Default);
    }

    /// <summary>
    /// Provides JSON deserialization for a payload being read from a stream, without buffering it into a string first.
    /// </summary>
    ///
    /// <param name="stream">
    /// The stream to read the JSON payload from. Read from its current position and left open, so the caller keeps ownership
    /// of both the position and the disposal.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    extension(Stream stream)
    {
        /// <summary>
        /// Asynchronously deserializes the stream into <typeparamref name="T"/> using the supplied options.
        /// </summary>
        ///
        /// <typeparam name="T">The type to bind the payload to.</typeparam>
        /// <param name="options">The serializer options to apply, in place of the package defaults.</param>
        ///
        /// <returns>
        /// A task producing the deserialized value, or <c>null</c> when the payload is the JSON literal <c>null</c>.
        /// </returns>
        ///
        /// <exception cref="JsonException">
        /// The stream does not contain valid JSON, carries data after the first document, or cannot bind to
        /// <typeparamref name="T"/>.
        /// </exception>
        ///
        /// <remarks>
        /// There is no <c>Try</c> counterpart for streams, because an <c>out</c> parameter cannot be used on an asynchronous
        /// method. Catch <see cref="JsonException"/> at the call site when malformed input is expected.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.1.0</since>
        public async Task<T?> DeserializeAsync<T>(JsonSerializerOptions options)
            => await JsonSerializer.DeserializeAsync<T>(stream, options);

        /// <summary>
        /// Asynchronously deserializes the stream into <typeparamref name="T"/> using the package defaults.
        /// </summary>
        ///
        /// <typeparam name="T">The type to bind the payload to.</typeparam>
        /// <param name="useDefaultOptions">
        /// Whether to apply the package defaults, which use camel-case property names. Pass <c>false</c> to fall back to the
        /// <see cref="System.Text.Json"/> defaults, which match property names exactly.
        /// </param>
        ///
        /// <returns>
        /// A task producing the deserialized value, or <c>null</c> when the payload is the JSON literal <c>null</c>.
        /// </returns>
        ///
        /// <exception cref="JsonException">
        /// The stream does not contain valid JSON, carries data after the first document, or cannot bind to
        /// <typeparamref name="T"/>.
        /// </exception>
        ///
        /// <remarks>
        /// There is no <c>Try</c> counterpart for streams, because an <c>out</c> parameter cannot be used on an asynchronous
        /// method. Catch <see cref="JsonException"/> at the call site when malformed input is expected.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.1.0</since>
        public async Task<T?> DeserializeAsync<T>(bool useDefaultOptions = true)
            => await JsonSerializer.DeserializeAsync<T>(stream, useDefaultOptions ? DefaultOptions : null);
    }
}
