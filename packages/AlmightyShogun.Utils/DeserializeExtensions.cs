using System.Text.Json;

namespace AlmightyShogun.Utils;

public static class DeserializeExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Deserializes the specified JSON string to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// 
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="options">The options for serialization.</param>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// 
    /// <returns>
    /// An object of type <typeparamref name="T"/>, or null if the deserialization fails.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static T? Deserialize<T>(this string json, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(json, options);
    }

    /// <summary>
    /// Deserializes the specified JSON string to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// 
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="useDefaultOptions">If the defaults <see cref="JsonSerializerOptions"/> from the library should be used.</param>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// 
    /// <returns>
    /// An object of type <typeparamref name="T"/>, or null if the deserialization fails.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static T? Deserialize<T>(this string json, bool useDefaultOptions = true)
    {
        return JsonSerializer.Deserialize<T>(json, useDefaultOptions ? JsonSerializerOptions : null);
    }
    
    /// <summary>
    /// Asynchronously deserializes the specified JSON stream to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// 
    /// <param name="stream">The JSON stream to deserialize.</param>
    /// <param name="options">The options for serialization.</param>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// 
    /// <returns>
    /// A task representing the asynchronous operation, with a result of type <typeparamref name="T"/>, or null if the deserialization fails.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static async Task<T?> DeserializeAsync<T>(this Stream stream, JsonSerializerOptions? options = null)
    {
        return await JsonSerializer.DeserializeAsync<T>(stream, options);
    }
    
    /// <summary>
    /// Asynchronously deserializes the specified JSON stream to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// 
    /// <param name="stream">The JSON stream to deserialize.</param>
    /// <param name="useDefaultOptions">If the defaults <see cref="JsonSerializerOptions"/> from the library should be used.</param>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// 
    /// <returns>
    /// A task representing the asynchronous operation, with a result of type <typeparamref name="T"/>, or null if the deserialization fails.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    public static async Task<T?> DeserializeAsync<T>(this Stream stream, bool useDefaultOptions = true)
    {
        return await JsonSerializer.DeserializeAsync<T>(stream, useDefaultOptions ? JsonSerializerOptions : null);
    }
}
