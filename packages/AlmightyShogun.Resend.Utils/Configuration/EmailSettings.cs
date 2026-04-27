namespace AlmightyShogun.Resend.Utils.Configuration;
public sealed record EmailSettings
{
    public required string ApiToken { get; init; }

    public required string BrandName { get; init; }

    public required string FromEmail { get; init; }

    public required string FromName { get; init; }

    public required string LogoUrl { get; init; }

    public required string AppUrl { get; init; }

    public required Dictionary<string, string> Links { get; init; }

    public required EmailTemplateSettings Template { get; init; }
}
