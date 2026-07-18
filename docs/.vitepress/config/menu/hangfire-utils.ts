import type { DefaultTheme } from 'vitepress'

export const hangfireUtils: DefaultTheme.SidebarItem[] = [
    {
        text: 'Hangfire Utils',
        items: [
            { text: 'Introduction', link: '/hangfire-utils/' },
            { text: 'Installation', link: '/hangfire-utils/installation' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddHangfire', link: '/hangfire-utils/extensions/add-hangfire' },
            { text: 'RegisterRecurringJobs', link: '/hangfire-utils/extensions/register-recurring-jobs' }
        ]
    },
    {
        text: 'Attributes',
        collapsed: false,
        items: [
            { text: 'RecurringJob', link: '/hangfire-utils/attributes/recurring-job-attribute' }
        ]
    },
    {
        text: 'Types',
        collapsed: false,
        items: [
            { text: 'RecurringJobBase', link: '/hangfire-utils/types/recurring-job-base' }
        ]
    },
];
