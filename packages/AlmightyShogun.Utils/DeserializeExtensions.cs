using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace AlmightyShogun.Utils;

/// <summary>
/// Provides JSON deserialization for strings and streams. A string is read through the <c>Try</c> shape, which reports
/// malformed input rather than throwing; a stream throws, because an <c>out</c> parameter cannot appear on an asynchronous
/// method. Options are optional on both, and omitting them applies the package defaults.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class DeserializeExtensions
{
    /// <summary>
    /// Gets the package default JSON options, built from <see cref="JsonSerializerDefaults.Web"/> so a payload from an
    /// ASP.NET Core API binds on the rules that API itself applied: camel-case naming, case-insensitive property matching, and
    /// numbers accepted from JSON strings. The instance is read-only, so it can be shared across the process and passed to
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
    /// <exception cref="InvalidOperationException">
    /// The <see cref="JsonSerializer.IsReflectionEnabledByDefault"/> feature switch is off, which
    /// <see cref="JsonSerializerOptions.MakeReadOnly(bool)"/> documents as a failure. The switch is off only for a consumer
    /// that set it so explicitly. Because this runs from the initialiser of <see cref="DefaultOptions"/>, it surfaces as a
    /// <see cref="TypeInitializationException"/> on the first call that leaves the options unset.
    /// </exception>
    ///
    /// <remarks>
    /// The web preset is taken whole rather than reproduced property by property, so these defaults keep matching ASP.NET Core
    /// if a future runtime changes what the preset covers.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static JsonSerializerOptions CreateDefaultOptions()
    {
        JsonSerializerOptions options = new(JsonSerializerDefaults.Web);

        options.MakeReadOnly(true);

        return options;
    }

    /// <summary>
    /// Provides JSON deserialization for a payload already held in memory.
    /// </summary>
    ///
    /// <param name="json">
    /// The JSON text to read. Deserialized in full, so the whole document must be present; use the stream member
    /// <c>DeserializeAsync</c> to avoid materializing a large payload as a string first.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    extension(string json)
    {
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
        /// <param name="options">
        /// The serializer options to apply. Left unset, the package defaults are used, which bind an ASP.NET Core payload on
        /// the same rules the API serialized it with.
        /// </param>
        ///
        /// <returns><c>true</c> when a non-null value was read; otherwise <c>false</c>.</returns>
        ///
        /// <exception cref="ArgumentNullException">The receiver string is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">
        /// No converter exists for <typeparamref name="T"/> or for one of its serializable members.
        /// </exception>
        ///
        /// <remarks>
        /// Only <see cref="JsonException"/> is caught, so the failures listed above still propagate despite the <c>Try</c>
        /// shape, and a genuine programming error is not swallowed by a method whose name suggests it only reports success or
        /// failure.
        ///
        /// A payload that is the JSON literal <c>null</c> reports <c>false</c> rather than succeeding with a null value, so a
        /// <c>true</c> result always yields something usable. Nothing here distinguishes that case from malformed input; call
        /// <see cref="JsonSerializer"/> directly when the two have to be told apart.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
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
        /// <param name="options">
        /// The serializer options to apply. Left unset, the package defaults are used, which bind an ASP.NET Core payload on
        /// the same rules the API serialized it with.
        /// </param>
        /// <param name="cancellationToken">
        /// Stops the read part way through. The stream is left at wherever reading reached, so a cancelled call leaves it
        /// unusable for a second attempt unless the caller can rewind it.
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
        /// <exception cref="ArgumentNullException">The receiver stream is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">
        /// No converter exists for <typeparamref name="T"/> or for one of its serializable members.
        /// </exception>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was signalled during the read.</exception>
        ///
        /// <remarks>
        /// There is no <c>Try</c> counterpart for streams, because an <c>out</c> parameter cannot be used on an asynchronous
        /// method. Catch <see cref="JsonException"/> at the call site when malformed input is expected.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.1.0</since>
        public async Task<T?> DeserializeAsync<T>(
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default
        ) => await JsonSerializer.DeserializeAsync<T>(stream, options ?? DefaultOptions, cancellationToken);
    }
}
