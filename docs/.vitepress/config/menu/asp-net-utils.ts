import type { DefaultTheme } from 'vitepress'

export const aspNetUtils: DefaultTheme.SidebarItem[] = [
    {
        text: 'AspNet Utils',
        items: [
            { text: 'Introduction', link: '/asp-net-utils/' },
            { text: 'Installation', link: '/asp-net-utils/installation' },
            { text: 'Configuration', link: '/asp-net-utils/configuration' },
            { text: 'Localization', link: '/asp-net-utils/localization' },
            { text: 'Exceptions', link: '/asp-net-utils/exceptions' }
        ]
    },
    {
        text: 'Configuration',
        collapsed: false,
        items: [
            { text: 'HttpErrorSettings', link: '/asp-net-utils/configuration/http-error-settings' },
            { text: 'LocalizationSettings', link: '/asp-net-utils/configuration/localization-settings' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddCloudflareHeaders', link: '/asp-net-utils/extensions/add-cloudflare-headers' },
            { text: 'AddCorsPolicy', link: '/asp-net-utils/extensions/add-cors-policy' },
            { text: 'AddExceptionHandling', link: '/asp-net-utils/extensions/add-exception-handling' },
            { text: 'AddHttpErrorResponseFilter', link: '/asp-net-utils/extensions/add-http-error-response-filter' },
            { text: 'AddHttpErrorResponseWriter', link: '/asp-net-utils/extensions/add-http-error-response-writer' },
            { text: 'AddMessageLocalization', link: '/asp-net-utils/extensions/add-message-localization' },
            { text: 'AddSessionContextFilter', link: '/asp-net-utils/extensions/add-session-context-filter' },
            { text: 'DeleteCookies', link: '/asp-net-utils/extensions/delete-cookies' },
            { text: 'GetAcceptLanguage', link: '/asp-net-utils/extensions/get-accept-language' },
            { text: 'GetAcceptLanguages', link: '/asp-net-utils/extensions/get-accept-languages' },
            { text: 'GetContentLanguage', link: '/asp-net-utils/extensions/get-content-language' },
            { text: 'GetIpAddress', link: '/asp-net-utils/extensions/get-ip-address' },
            { text: 'GetSessionContext', link: '/asp-net-utils/extensions/get-session-context' },
            { text: 'GetUserAgent', link: '/asp-net-utils/extensions/get-user-agent' },
            { text: 'SetContentLanguage', link: '/asp-net-utils/extensions/set-content-language' },
            { text: 'UseHttpErrorResponses', link: '/asp-net-utils/extensions/use-http-error-responses' },
            { text: 'UseMessageLocalization', link: '/asp-net-utils/extensions/use-message-localization' }
        ]
    },
    {
        text: 'Handlers',
        collapsed: false,
        items: [
            { text: 'AppExceptionHandler', link: '/asp-net-utils/handlers/app-exception-handler' },
            { text: 'FrameworkExceptionHandler', link: '/asp-net-utils/handlers/framework-exception-handler' },
            { text: 'UnhandledExceptionHandler', link: '/asp-net-utils/handlers/unhandled-exception-handler' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'HttpErrorResponseWriter', link: '/asp-net-utils/services/http-error-response-writer' },
            { text: 'LanguageProvider', link: '/asp-net-utils/services/language-provider' },
            { text: 'MessageResolver', link: '/asp-net-utils/services/message-resolver' }
        ]
    },
    {
        text: 'Utilities',
        collapsed: false,
        items: [
            { text: 'Cloudflare', link: '/asp-net-utils/utilities/cloudflare' },
            { text: 'HttpErrorResult', link: '/asp-net-utils/utilities/http-error-result' }
        ]
    },
    {
        text: 'Records',
        collapsed: false,
        items: [
            { text: 'HttpErrorResponse', link: '/asp-net-utils/records/http-error-response' },
            { text: 'HttpProblemDetails', link: '/asp-net-utils/records/http-problem-details' },
            { text: 'SessionContext', link: '/asp-net-utils/records/session-context' },
            { text: 'UserAgent', link: '/asp-net-utils/records/user-agent' }
        ]
    },
];
