import type { DefaultTheme } from 'vitepress'

export const aspNetAuth: DefaultTheme.SidebarItem[] = [
    {
        text: 'ASP.NET Auth',
        items: [
            { text: 'Introduction', link: '/asp-net-auth/' },
            { text: 'Installation', link: '/asp-net-auth/installation' },
            { text: 'Configuration', link: '/asp-net-auth/configuration' },
            { text: 'Exceptions', link: '/asp-net-auth/exceptions' },
            { text: 'Localization', link: '/asp-net-auth/localization' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddAuth', link: '/asp-net-auth/extensions/add-auth' },
            { text: 'DeleteAuthCookies', link: '/asp-net-auth/extensions/delete-auth-cookies' },
            { text: 'GetCurrentUserId', link: '/asp-net-auth/extensions/get-current-user-id' },
            { text: 'GetRefreshTokenCookie', link: '/asp-net-auth/extensions/get-refresh-token-cookie' },
            { text: 'SetRefreshTokenCookie', link: '/asp-net-auth/extensions/set-refresh-token-cookie' },
            { text: 'TryGetCurrentUserId', link: '/asp-net-auth/extensions/try-get-current-user-id' },
            { text: 'TryGetRefreshTokenCookie', link: '/asp-net-auth/extensions/try-get-refresh-token-cookie' }
        ]
    },
    {
        text: 'Attributes',
        collapsed: false,
        items: [
            { text: 'AuthPermission', link: '/asp-net-auth/attributes/auth-permission' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'AppHostResolver', link: '/asp-net-auth/services/app-host-resolver' },
            { text: 'AuthTokenGenerator', link: '/asp-net-auth/services/auth-token-generator' }
        ]
    },
    {
        text: 'Records',
        collapsed: false,
        items: [
            { text: 'AuthToken', link: '/asp-net-auth/records/auth-token' }
        ]
    },
    {
        text: 'Constants',
        collapsed: false,
        items: [
            { text: 'AuthClaimTypes', link: '/asp-net-auth/constants/auth-claim-types' },
            { text: 'AuthPolicies', link: '/asp-net-auth/constants/auth-policies' },
            { text: 'CookieNames', link: '/asp-net-auth/constants/cookie-names' }
        ]
    },
];
