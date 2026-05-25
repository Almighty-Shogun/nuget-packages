import type { DefaultTheme } from 'vitepress'

export const aspNetUtils: DefaultTheme.SidebarItem[] = [
    {
        text: 'ASP.NET Utils',
        items: [
            { text: 'Introduction', link: '/asp-net-utils/' },
            { text: 'Installation', link: '/asp-net-utils/installation' },
            { text: 'Configuration', link: '/asp-net-utils/configuration' }
        ]
    },
    {
        text: 'Classes',
        collapsed: false,
        items: [
            {
                text: 'UserAgentParser',
                link: '/asp-net-utils/classes/user-agent-parser/',
                collapsed: true,
                items: [
                    { text: 'Parse', link: '/asp-net-utils/classes/user-agent-parser/parse' }
                ]
            }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddActionFilters', link: '/asp-net-utils/extensions/add-action-filters' },
            { text: 'AddAllowedOrigins', link: '/asp-net-utils/extensions/add-allowed-origins' },
            { text: 'DeleteCookies', link: '/asp-net-utils/extensions/delete-cookies' },
            { text: 'GetSessionContext', link: '/asp-net-utils/extensions/get-session-context' }
        ]
    },
    {
        text: 'Records',
        collapsed: false,
        items: [
            { text: 'SessionContext', link: '/asp-net-utils/records/session-context' },
            { text: 'UserAgent', link: '/asp-net-utils/records/user-agent' }
        ]
    },
];
