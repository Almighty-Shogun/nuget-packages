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
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'ApplyAutoInclude', link: '/ef-core-utils/extensions/apply-auto-include' },
            { text: 'ApplyEnumAsString', link: '/ef-core-utils/extensions/apply-enum-as-string' },
            { text: 'ApplyIndex', link: '/ef-core-utils/extensions/apply-index' },
            { text: 'ApplyManyToMany', link: '/ef-core-utils/extensions/apply-many-to-many' },
            { text: 'ApplyManyToOne', link: '/ef-core-utils/extensions/apply-many-to-one' },
            { text: 'ApplyOneToMany', link: '/ef-core-utils/extensions/apply-one-to-many' },
            { text: 'ApplyOneToOne', link: '/ef-core-utils/extensions/apply-one-to-one' },
            { text: 'ApplyOwned', link: '/ef-core-utils/extensions/apply-owned' }
        ]
    },
];
