using System.Net;
using System.Net.Sockets;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires text to be an IP address, of either family or of one specifically.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
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
            _ => false
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
    /// <since>Unreleased</since>
    private string GetMessageKey() => mode switch
    {
        IpMode.Ipv4 => "validation.ip.ipv4",
        IpMode.Ipv6 => "validation.ip.ipv6",
        _ => "validation.ip"
    };
}
