import type { DefaultTheme } from 'vitepress'

export const aspNetJwtAuth: DefaultTheme.SidebarItem[] = [
    {
        text: 'ASP.NET JWT Auth',
        items: [
            { text: 'Introduction', link: '/asp-net-jwt-auth/' },
            { text: 'Installation', link: '/asp-net-jwt-auth/installation' },
            { text: 'Configuration', link: '/asp-net-jwt-auth/configuration' }
        ]
    },
    {
        text: 'Configuration',
        collapsed: false,
        items: [
            { text: 'AuthSettings', link: '/asp-net-jwt-auth/configuration/auth-settings' }
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
            { text: 'TryGetRefreshTokenCookie', link: '/asp-net-jwt-auth/extensions/try-get-refresh-token-cookie' }
        ]
    },
    {
        text: 'Attributes',
        collapsed: false,
        items: [
            { text: 'AuthPermission', link: '/asp-net-jwt-auth/attributes/auth-permission-attribute' },
            { text: 'RequireRefreshToken', link: '/asp-net-jwt-auth/attributes/require-refresh-token-attribute' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'AppHostResolver', link: '/asp-net-jwt-auth/services/app-host-resolver' }
        ]
    },
    {
        text: 'Constants',
        collapsed: false,
        items: [
            { text: 'CookieNames', link: '/asp-net-jwt-auth/constants/cookie-names' }
        ]
    },
];
