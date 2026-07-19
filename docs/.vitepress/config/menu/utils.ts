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
            { text: 'Deserialize', link: '/utils/extensions/deserialize' },
            { text: 'DeserializeAsync', link: '/utils/extensions/deserialize-async' },
            { text: 'RegisterOnInherit', link: '/utils/extensions/register-on-inherit' },
            { text: 'Serialize', link: '/utils/extensions/serialize' },
            { text: 'SerializeAsync', link: '/utils/extensions/serialize-async' }
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
        text: 'Types',
        collapsed: false,
        items: [
            { text: 'ApplicationUtils', link: '/utils/types/application-utils' }
        ]
    },
];
