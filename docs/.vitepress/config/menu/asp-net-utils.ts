import type { DefaultTheme } from 'vitepress'

export const aspNetUtils: DefaultTheme.SidebarItem[] = [
    {
        text: 'AspNet Utils',
        items: [
            { text: 'Introduction', link: '/asp-net-utils/' },
            { text: 'Installation', link: '/asp-net-utils/installation' },
            { text: 'Configuration', link: '/asp-net-utils/configuration' },
            { text: 'Exceptions', link: '/asp-net-utils/exceptions' },
            { text: 'HTTP Error Messages', link: '/asp-net-utils/http-error-messages' }
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
            { text: 'DeleteCookies', link: '/asp-net-utils/extensions/delete-cookies' },
            { text: 'GetIpAddress', link: '/asp-net-utils/extensions/get-ip-address' },
            { text: 'GetSessionContext', link: '/asp-net-utils/extensions/get-session-context' },
            { text: 'GetUserAgent', link: '/asp-net-utils/extensions/get-user-agent' },
            { text: 'UseHttpErrorResponses', link: '/asp-net-utils/extensions/use-http-error-responses' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'HttpErrorResponseWriter', link: '/asp-net-utils/services/http-error-response-writer' }
        ]
    },
    {
        text: 'Utilities',
        collapsed: false,
        items: [
            { text: 'CloudflareDefaults', link: '/asp-net-utils/utilities/cloudflare-defaults' },
            { text: 'HttpErrorResult', link: '/asp-net-utils/utilities/http-error-result' }
        ]
    },
    {
        text: 'Records',
        collapsed: false,
        items: [
            { text: 'ErrorMapping', link: '/asp-net-utils/records/error-mapping' },
            { text: 'HttpErrorResponse', link: '/asp-net-utils/records/http-error-response' },
            { text: 'SessionContext', link: '/asp-net-utils/records/session-context' },
            { text: 'UserAgent', link: '/asp-net-utils/records/user-agent' }
        ]
    },
];
