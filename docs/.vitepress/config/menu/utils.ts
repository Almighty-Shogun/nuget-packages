import type { DefaultTheme } from 'vitepress'

export const utils: DefaultTheme.SidebarItem[] = [
    {
        text: 'Utils',
        items: [
            { text: 'Introduction', link: '/utils/' },
            { text: 'Installation', link: '/utils/installation' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddConfiguration', link: '/utils/extensions/add-configuration' },
            { text: 'AddService', link: '/utils/extensions/add-service' },
            { text: 'DeserializeAsync', link: '/utils/extensions/deserialize-async' },
            { text: 'RegisterOnInherit', link: '/utils/extensions/register-on-inherit' },
            { text: 'TryDeserialize', link: '/utils/extensions/try-deserialize' }
        ]
    },
    {
        text: 'Attributes',
        collapsed: false,
        items: [
            { text: 'SkipAutoRegistration', link: '/utils/attributes/skip-auto-registration' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'ServiceRegistry', link: '/utils/services/service-registry' }
        ]
    },
    {
        text: 'Utilities',
        collapsed: false,
        items: [
            { text: 'ConsoleUtils', link: '/utils/utilities/console-utils' },
            { text: 'TypeDiscovery', link: '/utils/utilities/type-discovery' }
        ]
    },
];
