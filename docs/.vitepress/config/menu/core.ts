import type { DefaultTheme } from 'vitepress'

export const core: DefaultTheme.SidebarItem[] = [
    {
        text: 'Core',
        items: [
            { text: 'Introduction', link: '/core/' },
            { text: 'Installation', link: '/core/installation' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddConfiguration', link: '/core/extensions/add-configuration' },
            { text: 'AddService', link: '/core/extensions/add-service' },
            { text: 'DeserializeAsync', link: '/core/extensions/deserialize-async' },
            { text: 'RegisterOnInherit', link: '/core/extensions/register-on-inherit' },
            { text: 'ReplaceService', link: '/core/extensions/replace-service' },
            { text: 'TryDeserialize', link: '/core/extensions/try-deserialize' }
        ]
    },
    {
        text: 'Attributes',
        collapsed: false,
        items: [
            { text: 'SkipAutoRegistration', link: '/core/attributes/skip-auto-registration' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'ServiceRegistry', link: '/core/services/service-registry' }
        ]
    },
    {
        text: 'Utilities',
        collapsed: false,
        items: [
            { text: 'ConsoleUtils', link: '/core/utilities/console-utils' },
            { text: 'TypeDiscovery', link: '/core/utilities/type-discovery' }
        ]
    },
];
