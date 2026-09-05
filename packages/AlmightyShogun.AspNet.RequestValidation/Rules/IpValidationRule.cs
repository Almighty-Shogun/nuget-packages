using System.Net;
using System.Net.Sockets;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires text to be an IP address, of either family or of one specifically. An absent or empty value passes without being checked, so
/// the rule never implies the field is required.
/// </summary>
///
/// <typeparam name="TRequest">The request type the rule is declared on, though only the bound value decides the outcome.</typeparam>
/// <typeparam name="TProperty">The bound property's type; a non-empty value that cannot be read as text fails.</typeparam>
/// <param name="mode">Which address family to insist on, which also decides the message a failure reports.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class IpValidationRule<TRequest, TProperty>(
    IpMode mode
) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (!ValidationValue.TryGetText(value, out string text) || !IPAddress.TryParse(text, out IPAddress? address))
            return ValueTask.FromResult(ValidationRuleResult.Failure(GetMessageKey()));

        bool isValid = mode switch
        {
            IpMode.Any => true,
            IpMode.Ipv4 => address.AddressFamily == AddressFamily.InterNetwork,
            IpMode.Ipv6 => address.AddressFamily == AddressFamily.InterNetworkV6,
            _ => throw new InvalidOperationException($"Unsupported IpMode value '{mode}'.")
        };

        return ValueTask.FromResult(isValid ? ValidationRuleResult.Success() : ValidationRuleResult.Failure(GetMessageKey()));
    }

    /// <summary>
    /// Maps the configured mode onto the message key its failure reports, so one rule class serves every spelling of its family without
    /// each needing a class of its own.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private string GetMessageKey() => mode switch
    {
        IpMode.Ipv4 => "validation.ip.ipv4",
        IpMode.Ipv6 => "validation.ip.ipv6",
        IpMode.Any => "validation.ip",
        _ => throw new InvalidOperationException($"Unsupported IpMode value '{mode}'.")
    };
}
