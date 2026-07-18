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
            { text: 'AddApiAuth', link: '/asp-net-jwt-auth/extensions/add-api-auth' },
            { text: 'AddJwtBearerAuthentication', link: '/asp-net-jwt-auth/extensions/add-jwt-bearer-authentication' },
            { text: 'DeleteAuthCookies', link: '/asp-net-jwt-auth/extensions/delete-auth-cookies' },
            { text: 'GetCurrentUserId', link: '/asp-net-jwt-auth/extensions/get-current-user-id' },
            { text: 'GetRefreshTokenCookie', link: '/asp-net-jwt-auth/extensions/get-refresh-token-cookie' },
            { text: 'SetRefreshTokenCookie', link: '/asp-net-jwt-auth/extensions/set-refresh-token-cookie' }
        ]
    },
    {
        text: 'Attributes',
        collapsed: false,
        items: [
            { text: 'AuthPermission', link: '/asp-net-jwt-auth/attributes/auth-permission-attribute' }
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
