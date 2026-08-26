import type { DefaultTheme } from 'vitepress'

export const efCoreModelBuilding: DefaultTheme.SidebarItem[] = [
    {
        text: 'EF Core Model Building',
        items: [
            { text: 'Introduction', link: '/ef-core-model-building/' },
            { text: 'Installation', link: '/ef-core-model-building/installation' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'ApplyAutoInclude', link: '/ef-core-model-building/extensions/apply-auto-include' },
            { text: 'ApplyEnumAsString', link: '/ef-core-model-building/extensions/apply-enum-as-string' },
            { text: 'ApplyIndex', link: '/ef-core-model-building/extensions/apply-index' },
            { text: 'ApplyManyToMany', link: '/ef-core-model-building/extensions/apply-many-to-many' },
            { text: 'ApplyManyToOne', link: '/ef-core-model-building/extensions/apply-many-to-one' },
            { text: 'ApplyOneToMany', link: '/ef-core-model-building/extensions/apply-one-to-many' },
            { text: 'ApplyOneToOne', link: '/ef-core-model-building/extensions/apply-one-to-one' },
            { text: 'ApplyOwned', link: '/ef-core-model-building/extensions/apply-owned' },
            { text: 'ApplyUniqueIndex', link: '/ef-core-model-building/extensions/apply-unique-index' }
        ]
    },
];
