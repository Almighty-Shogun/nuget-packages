using System.Text.Json.Serialization;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// The RFC 9457 problem details body, written in place of <see cref="HttpErrorResponse"/> when
/// <see cref="HttpErrorSettings.UseProblemDetails"/> is enabled.
/// </summary>
///
/// <remarks>
/// Field for field this carries the same information under the names the specification mandates: <c>Title</c> holds the
/// error code and <c>Detail</c> the description. A client written against one shape cannot read the other, so the
/// setting is a deployment-time decision rather than something to vary per endpoint.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public record HttpProblemDetails
{
    /// <summary>
    /// Gets the URI reference identifying the problem type. Defaults to <c>about:blank</c>, which RFC 9457 defines as
    /// carrying no meaning beyond the status code.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string Type { get; init; } = "about:blank";

    /// <summary>
    /// Gets the short identifier for the problem type. Carries the package error code, so the machine-readable value
    /// survives the switch to problem details.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string Title { get; init; }

    /// <summary>
    /// Gets the status code, duplicated here as RFC 9457 requires, and always the same value as the response status.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required int Status { get; init; }

    /// <summary>
    /// Gets the human-readable explanation, localized the same way as its counterpart on
    /// <see cref="HttpErrorResponse"/>. Omitted from the payload when absent rather than serialized as null, which the
    /// specification allows and which keeps an error body from carrying an empty field.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Detail { get; init; }

    /// <summary>
    /// Gets the request path the problem occurred on, omitted from the payload when the path is unavailable. It is the
    /// raw path only, without the query string, so a value carried in the query is never echoed back.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Instance { get; init; }
}
