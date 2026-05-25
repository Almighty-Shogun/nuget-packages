import type { DefaultTheme } from 'vitepress'

export const logging: DefaultTheme.SidebarItem[] = [
    {
        text: 'Logging',
        items: [
            { text: 'Introduction', link: '/logging/' },
            { text: 'Installation', link: '/logging/installation' },
            { text: 'Configuration', link: '/logging/configuration' },
            { text: 'Formatter', link: '/logging/formatter' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            {
                text: 'AddCustomLogging',
                link: '/logging/extensions/add-custom-logging/',
                collapsed: true,
                items: [
                    { text: 'IHostBuilder', link: '/logging/extensions/add-custom-logging/host-builder' },
                    { text: 'IServiceCollection', link: '/logging/extensions/add-custom-logging/service-collection' }
                ]
            }
        ]
    },
];
