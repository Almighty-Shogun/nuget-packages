import type { DefaultTheme } from 'vitepress'

export const aspNetUtils: DefaultTheme.SidebarItem[] = [
    {
        text: 'ASP.NET Utils',
        items: [
            { text: 'Introduction', link: '/asp-net-utils/' },
            { text: 'Installation', link: '/asp-net-utils/installation' },
            { text: 'Configuration', link: '/asp-net-utils/configuration' },
            { text: 'Localization', link: '/asp-net-utils/localization' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddActionFilters', link: '/asp-net-utils/extensions/add-action-filters' },
            { text: 'AddAllowedOrigins', link: '/asp-net-utils/extensions/add-allowed-origins' },
            { text: 'AddHttpErrorResponses', link: '/asp-net-utils/extensions/add-http-error-responses' },
            { text: 'DeleteCookies', link: '/asp-net-utils/extensions/delete-cookies' },
            { text: 'GetAcceptLanguage', link: '/asp-net-utils/extensions/get-accept-language' },
            { text: 'GetContentLanguage', link: '/asp-net-utils/extensions/get-content-language' },
            { text: 'GetSessionContext', link: '/asp-net-utils/extensions/get-session-context' },
            { text: 'GetUserAgent', link: '/asp-net-utils/extensions/get-user-agent' },
            { text: 'SetContentLanguage', link: '/asp-net-utils/extensions/set-content-language' },
            { text: 'UseHttpErrorResponses', link: '/asp-net-utils/extensions/use-http-error-responses' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'MessageResolver', link: '/asp-net-utils/services/message-resolver' }
        ]
    },
    {
        text: 'Types',
        collapsed: false,
        items: [
            { text: 'HttpErrorException', link: '/asp-net-utils/types/http-error-exception' },
            { text: 'HttpErrorResult', link: '/asp-net-utils/types/http-error-result' }
        ]
    },
    {
        text: 'Records',
        collapsed: false,
        items: [
            { text: 'HttpErrorResponse', link: '/asp-net-utils/records/http-error-response' },
            { text: 'SessionContext', link: '/asp-net-utils/records/session-context' },
            { text: 'UserAgent', link: '/asp-net-utils/records/user-agent' }
        ]
    },
];
