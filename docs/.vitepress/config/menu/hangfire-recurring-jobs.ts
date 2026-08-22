import type { DefaultTheme } from 'vitepress'

export const hangfireRecurringJobs: DefaultTheme.SidebarItem[] = [
    {
        text: 'Hangfire Recurring Jobs',
        items: [
            { text: 'Introduction', link: '/hangfire-recurring-jobs/' },
            { text: 'Installation', link: '/hangfire-recurring-jobs/installation' },
            { text: 'Configuration', link: '/hangfire-recurring-jobs/configuration' }
        ]
    },
    {
        text: 'Configuration',
        collapsed: false,
        items: [
            { text: 'RecurringJobSettings', link: '/hangfire-recurring-jobs/configuration/recurring-job-settings' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddCustomHangfire', link: '/hangfire-recurring-jobs/extensions/add-custom-hangfire' },
            { text: 'RegisterRecurringJobs', link: '/hangfire-recurring-jobs/extensions/register-recurring-jobs' }
        ]
    },
    {
        text: 'Attributes',
        collapsed: false,
        items: [
            { text: 'RecurringJob', link: '/hangfire-recurring-jobs/attributes/recurring-job-attribute' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'RecurringJobRegistry', link: '/hangfire-recurring-jobs/services/recurring-job-registry' }
        ]
    },
    {
        text: 'Types',
        collapsed: false,
        items: [
            { text: 'RecurringJob', link: '/hangfire-recurring-jobs/types/recurring-job' }
        ]
    },
    {
        text: 'Records',
        collapsed: false,
        items: [
            { text: 'RecurringJobInfo', link: '/hangfire-recurring-jobs/records/recurring-job-info' }
        ]
    },
    {
        text: 'Constants',
        collapsed: false,
        items: [
            { text: 'CronSchedules', link: '/hangfire-recurring-jobs/constants/cron-schedules' }
        ]
    },
];
