namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Represents cached messages and their cache version.
/// </summary>
///
/// <param name="Version"> The cache version. </param>
/// <param name="Messages">The cached messages.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed record CachedMessages(long Version, IReadOnlyDictionary<string, string> Messages);
