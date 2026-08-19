import type { DefaultTheme } from 'vitepress'

export const serilog: DefaultTheme.SidebarItem[] = [
    {
        text: 'Serilog',
        items: [
            { text: 'Introduction', link: '/serilog/' },
            { text: 'Installation', link: '/serilog/installation' },
            { text: 'Configuration', link: '/serilog/configuration' },
            { text: 'Formatter', link: '/serilog/formatter' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddCustomLogging', link: '/serilog/extensions/add-custom-logging' }
        ]
    },
];
