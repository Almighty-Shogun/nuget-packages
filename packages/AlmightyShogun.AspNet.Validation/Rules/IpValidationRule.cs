using System.Net;
using System.Net.Sockets;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates IP address constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class IpValidationRule<TRequest, TProperty>(IpMode mode) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
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

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey()));
    }

    /// <summary>
    /// Gets the validation message key for the configured IP mode.
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
