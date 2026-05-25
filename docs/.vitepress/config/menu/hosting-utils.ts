import type { DefaultTheme } from 'vitepress'

export const hostingUtils: DefaultTheme.SidebarItem[] = [
    {
        text: 'Hosting Utils',
        items: [
            { text: 'Introduction', link: '/hosting-utils/' },
            { text: 'Installation', link: '/hosting-utils/installation' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'ConfigureHost', link: '/hosting-utils/extensions/configure-host' },
            { text: 'UseCustomConsoleLifetime', link: '/hosting-utils/extensions/use-custom-console-lifetime' }
        ]
    },
];
