namespace AlmightyShogun.Resend.Utils.Configuration;
public sealed record EmailTemplateSettings
{
    public required string CopyrightTextTemplate { get; init; }

    public required string FooterLinkText { get; init; }

    public required string IgnoreText { get; init; }
}
