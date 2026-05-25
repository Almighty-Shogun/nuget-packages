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
        text: 'Attributes',
        collapsed: false,
        items: [
            { text: 'RecurringJobAttribute', link: '/hangfire-utils/attributes/recurring-job-attribute' }
        ]
    },
    {
        text: 'Classes',
        collapsed: false,
        items: [
            {
                text: 'HangfireUtils',
                link: '/hangfire-utils/classes/hangfire-utils/',
                collapsed: true,
                items: [
                    { text: 'GetRecurringJobs', link: '/hangfire-utils/classes/hangfire-utils/get-recurring-jobs' }
                ]
            },
            {
                text: 'JobSchedulerStartup',
                link: '/hangfire-utils/classes/job-scheduler-startup/',
                collapsed: true,
                items: [
                    { text: 'StartAsync', link: '/hangfire-utils/classes/job-scheduler-startup/start-async' },
                    { text: 'StopAsync', link: '/hangfire-utils/classes/job-scheduler-startup/stop-async' }
                ]
            }
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
        text: 'Interfaces',
        collapsed: false,
        items: [
            {
                text: 'IRecurringJob',
                link: '/hangfire-utils/interfaces/irecurring-job/',
                collapsed: true,
                items: [
                    { text: 'RunAsync', link: '/hangfire-utils/interfaces/irecurring-job/run-async' }
                ]
            }
        ]
    },
    {
        text: 'Records',
        collapsed: false,
        items: [
            { text: 'RecurringJob', link: '/hangfire-utils/records/recurring-job' }
        ]
    },
];
