import type { DefaultTheme } from 'vitepress'

export const aspNetJwtAuth: DefaultTheme.SidebarItem[] = [
    {
        text: 'ASP.NET JWT Auth',
        items: [
            { text: 'Introduction', link: '/asp-net-jwt-auth/' },
            { text: 'Installation', link: '/asp-net-jwt-auth/installation' },
            { text: 'Configuration', link: '/asp-net-jwt-auth/configuration' },
            { text: 'Exceptions', link: '/asp-net-jwt-auth/exceptions' },
            { text: 'Localization', link: '/asp-net-jwt-auth/localization' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddJwtAuth', link: '/asp-net-jwt-auth/extensions/add-jwt-auth' },
            { text: 'DeleteAuthCookies', link: '/asp-net-jwt-auth/extensions/delete-auth-cookies' },
            { text: 'GetCurrentUserId', link: '/asp-net-jwt-auth/extensions/get-current-user-id' },
            { text: 'GetRefreshTokenCookie', link: '/asp-net-jwt-auth/extensions/get-refresh-token-cookie' },
            { text: 'SetRefreshTokenCookie', link: '/asp-net-jwt-auth/extensions/set-refresh-token-cookie' },
            { text: 'TryGetCurrentUserId', link: '/asp-net-jwt-auth/extensions/try-get-current-user-id' },
            { text: 'TryGetRefreshTokenCookie', link: '/asp-net-jwt-auth/extensions/try-get-refresh-token-cookie' }
        ]
    },
    {
        text: 'Attributes',
        collapsed: false,
        items: [
            { text: 'AuthPermission', link: '/asp-net-jwt-auth/attributes/auth-permission' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'AppHostResolver', link: '/asp-net-jwt-auth/services/app-host-resolver' },
            { text: 'AuthTokenGenerator', link: '/asp-net-jwt-auth/services/auth-token-generator' }
        ]
    },
    {
        text: 'Records',
        collapsed: false,
        items: [
            { text: 'AuthToken', link: '/asp-net-jwt-auth/records/auth-token' }
        ]
    },
    {
        text: 'Constants',
        collapsed: false,
        items: [
            { text: 'AuthClaimTypes', link: '/asp-net-jwt-auth/constants/auth-claim-types' },
            { text: 'AuthPolicies', link: '/asp-net-jwt-auth/constants/auth-policies' },
            { text: 'CookieNames', link: '/asp-net-jwt-auth/constants/cookie-names' }
        ]
    },
];
