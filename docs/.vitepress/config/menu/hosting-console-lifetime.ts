import type { DefaultTheme } from 'vitepress'

export const hostingConsoleLifetime: DefaultTheme.SidebarItem[] = [
    {
        text: 'Hosting Console Lifetime',
        items: [
            { text: 'Introduction', link: '/hosting-console-lifetime/' },
            { text: 'Installation', link: '/hosting-console-lifetime/installation' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'ConfigureHostOptions', link: '/hosting-console-lifetime/extensions/configure-host-options' },
            { text: 'UseCustomConsoleLifetime', link: '/hosting-console-lifetime/extensions/use-custom-console-lifetime' }
        ]
    },
];
