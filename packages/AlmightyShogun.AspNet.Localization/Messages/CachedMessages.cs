namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// One language's loaded messages, paired with the cache generation its files were read under so
/// <see cref="JsonMessageStore"/> can recognize an entry as superseded without the invalidating side having to find and
/// remove it.
/// </summary>
///
/// <param name="Version">
/// The store's cache generation as it stood when the load began. A reader compares it against the current generation
/// and reloads instead of trusting the entry when the two differ.
/// </param>
/// <param name="Messages">The flattened messages the load produced, keyed by their dot-separated key.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed record CachedMessages(long Version, IReadOnlyDictionary<string, string> Messages);
