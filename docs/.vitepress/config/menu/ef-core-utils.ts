import type { DefaultTheme } from 'vitepress'

export const efCoreUtils: DefaultTheme.SidebarItem[] = [
    {
        text: 'Entity Framework Core Utils',
        items: [
            { text: 'Introduction', link: '/ef-core-utils/' },
            { text: 'Installation', link: '/ef-core-utils/installation' }
        ]
    },
    {
        text: 'Classes',
        collapsed: false,
        items: [
            {
                text: 'ModelBuilderExtensions',
                link: '/ef-core-utils/classes/model-builder-extensions/',
                collapsed: true,
                items: [
                    { text: 'ApplyAutoInclude', link: '/ef-core-utils/classes/model-builder-extensions/apply-auto-include' },
                    { text: 'ApplyIndex', link: '/ef-core-utils/classes/model-builder-extensions/apply-index' },
                    { text: 'ApplyManyToOne', link: '/ef-core-utils/classes/model-builder-extensions/apply-many-to-one' },
                    { text: 'ApplyOneToMany', link: '/ef-core-utils/classes/model-builder-extensions/apply-one-to-many' },
                    { text: 'ApplyOneToOne', link: '/ef-core-utils/classes/model-builder-extensions/apply-one-to-one' }
                ]
            }
        ]
    },
];
