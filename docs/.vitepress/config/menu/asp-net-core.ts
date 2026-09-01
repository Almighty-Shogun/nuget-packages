import type { DefaultTheme } from 'vitepress'

export const aspNetCore: DefaultTheme.SidebarItem[] = [
    {
        text: 'ASP.NET Core',
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
            { text: 'AddHttpErrorResponseWriter', link: '/asp-net-core/extensions/add-http-error-response-writer' },
            { text: 'DeleteCookies', link: '/asp-net-core/extensions/delete-cookies' },
            { text: 'GetClientContext', link: '/asp-net-core/extensions/get-client-context' },
            { text: 'GetIpAddress', link: '/asp-net-core/extensions/get-ip-address' },
            { text: 'GetUserAgent', link: '/asp-net-core/extensions/get-user-agent' },
            { text: 'SetClientContext', link: '/asp-net-core/extensions/set-client-context' },
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
            { text: 'CloudflareDefaults', link: '/asp-net-core/utilities/cloudflare-defaults' }
        ]
    },
    {
        text: 'Types',
        collapsed: false,
        items: [
            { text: 'HttpErrorResult', link: '/asp-net-core/types/http-error-result' }
        ]
    },
    {
        text: 'Records',
        collapsed: false,
        items: [
            { text: 'ClientContext', link: '/asp-net-core/records/client-context' },
            { text: 'ErrorMapping', link: '/asp-net-core/records/error-mapping' },
            { text: 'HttpErrorResponse', link: '/asp-net-core/records/http-error-response' },
            { text: 'UserAgent', link: '/asp-net-core/records/user-agent' }
        ]
    },
];
