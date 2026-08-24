import type { DefaultTheme } from 'vitepress'

export const aspNetCore: DefaultTheme.SidebarItem[] = [
    {
        text: 'AspNet Core',
        items: [
            { text: 'Introduction', link: '/asp-net-core/' },
            { text: 'Installation', link: '/asp-net-core/installation' },
            { text: 'Configuration', link: '/asp-net-core/configuration' },
            { text: 'Exceptions', link: '/asp-net-core/exceptions' },
            { text: 'HTTP Error Messages', link: '/asp-net-core/http-error-messages' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddCloudflareHeaders', link: '/asp-net-core/extensions/add-cloudflare-headers' },
            { text: 'AddCorsPolicy', link: '/asp-net-core/extensions/add-cors-policy' },
            { text: 'AddExceptionHandling', link: '/asp-net-core/extensions/add-exception-handling' },
            { text: 'AddHttpErrorResponseFilter', link: '/asp-net-core/extensions/add-http-error-response-filter' },
            { text: 'AddHttpErrorResponseWriter', link: '/asp-net-core/extensions/add-http-error-response-writer' },
            { text: 'DeleteCookies', link: '/asp-net-core/extensions/delete-cookies' },
            { text: 'GetIpAddress', link: '/asp-net-core/extensions/get-ip-address' },
            { text: 'GetSessionContext', link: '/asp-net-core/extensions/get-session-context' },
            { text: 'GetUserAgent', link: '/asp-net-core/extensions/get-user-agent' },
            { text: 'UseHttpErrorResponses', link: '/asp-net-core/extensions/use-http-error-responses' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'HttpErrorResponseWriter', link: '/asp-net-core/services/http-error-response-writer' }
        ]
    },
    {
        text: 'Utilities',
        collapsed: false,
        items: [
            { text: 'CloudflareDefaults', link: '/asp-net-core/utilities/cloudflare-defaults' },
            { text: 'HttpErrorResult', link: '/asp-net-core/utilities/http-error-result' }
        ]
    },
    {
        text: 'Records',
        collapsed: false,
        items: [
            { text: 'ErrorMapping', link: '/asp-net-core/records/error-mapping' },
            { text: 'HttpErrorResponse', link: '/asp-net-core/records/http-error-response' },
            { text: 'SessionContext', link: '/asp-net-core/records/session-context' },
            { text: 'UserAgent', link: '/asp-net-core/records/user-agent' }
        ]
    },
];
