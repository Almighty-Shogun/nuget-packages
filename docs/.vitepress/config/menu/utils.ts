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
        text: 'Classes',
        collapsed: false,
        items: [
            {
                text: 'ApplicationUtils',
                link: '/utils/classes/application-utils/',
                collapsed: true,
                items: [
                    { text: 'Title', link: '/utils/classes/application-utils/title' },
                    { text: 'GetOnInherit', link: '/utils/classes/application-utils/get-on-inherit' },
                    { text: 'PreventCancellation', link: '/utils/classes/application-utils/prevent-cancellation' }
                ]
            }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            {
                text: 'DeserializeExtensions',
                link: '/utils/extensions/deserialize-extensions/',
                collapsed: true,
                items: [
                    { text: 'Deserialize', link: '/utils/extensions/deserialize-extensions/deserialize' },
                    { text: 'DeserializeAsync', link: '/utils/extensions/deserialize-extensions/deserialize-async' }
                ]
            },
            {
                text: 'ServiceCollectionExtensions',
                link: '/utils/extensions/service-collection-extensions/',
                collapsed: true,
                items: [
                    { text: 'AddService', link: '/utils/extensions/service-collection-extensions/add-service' },
                    { text: 'AddConfiguration', link: '/utils/extensions/service-collection-extensions/add-configuration' },
                    { text: 'RegisterOnInherit', link: '/utils/extensions/service-collection-extensions/register-on-inherit' }
                ]
            }
        ]
    },
    {
        text: 'Interfaces',
        collapsed: false,
        items: [
            {
                text: 'IService',
                link: '/utils/interfaces/iservice/',
                collapsed: true,
                items: [
                    { text: 'ConfigureService', link: '/utils/interfaces/iservice/configure-service' }
                ]
            }
        ]
    },
];
