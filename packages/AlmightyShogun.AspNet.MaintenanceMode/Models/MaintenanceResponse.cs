namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// The body the maintenance path returns. Separate from the internal state so the file's own bookkeeping, such as the allow lists, is never
/// exposed to whoever is being blocked.
/// </summary>
///
/// <param name="Message">The configured maintenance message.</param>
/// <param name="StartsAt">When the window starts, when scheduled ahead.</param>
/// <param name="EndsAt">When the window is expected to end.</param>
/// <param name="EnabledAt">When maintenance mode was enabled.</param>
///
/// <remarks>
/// This is a data payload that happens to carry a <c>503</c> status, not an error body, which is why it keeps its own shape rather than
/// going through the shared error response writer.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record MaintenanceResponse(string? Message, DateTimeOffset? StartsAt, DateTimeOffset? EndsAt, DateTimeOffset? EnabledAt);
