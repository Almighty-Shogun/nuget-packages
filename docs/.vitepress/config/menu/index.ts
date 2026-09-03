import type { DefaultTheme } from 'vitepress'
import { guide } from './guide'
import { aspNetCore } from './asp-net-core'
import { aspNetAuthCredentials } from './asp-net-auth-credentials'
import { aspNetAuth } from './asp-net-auth'
import { aspNetLocalization } from './asp-net-localization'
import { aspNetMaintenanceMode } from './asp-net-maintenance-mode'
import { aspNetRequestValidation } from './asp-net-request-validation'
import { consoleCommands } from './console-commands'
import { utils } from './utils'
import { efCoreModelBuilding } from './ef-core-model-building'
import { hangfireRecurringJobs } from './hangfire-recurring-jobs'
import { hostingConsoleLifetime } from './hosting-console-lifetime'
import { mailResend } from './mail-resend'
import { remoteCommands } from './remote-commands'
import { serilog } from './serilog'

export const nav: DefaultTheme.NavItem[] = [
    {
        text: 'Guide',
        link: '/guide/',
        activeMatch: '^/guide/',
    },
    {
        text: 'ASP.NET',
        activeMatch: '^/(asp-net-core|asp-net-auth-credentials|asp-net-auth|asp-net-localization|asp-net-maintenance-mode|asp-net-request-validation)/',
        items: [
            { text: 'Core', activeMatch: '^/asp-net-core/', link: '/asp-net-core/' },
            { text: 'Auth', activeMatch: '^/asp-net-auth/', link: '/asp-net-auth/' },
            { text: 'Localization', activeMatch: '^/asp-net-localization/', link: '/asp-net-localization/' },
            { text: 'Maintenance Mode', activeMatch: '^/asp-net-maintenance-mode/', link: '/asp-net-maintenance-mode/' },
            { text: 'Request Validation', activeMatch: '^/asp-net-request-validation/', link: '/asp-net-request-validation/' },
            {
                text: 'Login Systems',
                items: [
                    { text: 'Auth Credentials', activeMatch: '^/asp-net-auth-credentials/', link: '/asp-net-auth-credentials/' }
                ]
            }
        ]
    },
    {
        text: 'Operations',
        activeMatch: '^/(console-commands|hangfire-recurring-jobs|hosting-console-lifetime|remote-commands|serilog)/',
        items: [
            { text: 'Console Commands', activeMatch: '^/console-commands/', link: '/console-commands/' },
            { text: 'Hangfire Recurring Jobs', activeMatch: '^/hangfire-recurring-jobs/', link: '/hangfire-recurring-jobs/' },
            { text: 'Hosting Console Lifetime', activeMatch: '^/hosting-console-lifetime/', link: '/hosting-console-lifetime/' },
            { text: 'Remote Commands', activeMatch: '^/remote-commands/', link: '/remote-commands/' },
            { text: 'Serilog', activeMatch: '^/serilog/', link: '/serilog/' }
        ]
    },
    {
        text: 'Data',
        activeMatch: '^/(ef-core-model-building|mail-resend)/',
        items: [
            { text: 'EF Core Model Building', activeMatch: '^/ef-core-model-building/', link: '/ef-core-model-building/' },
            { text: 'Mail Resend', activeMatch: '^/mail-resend/', link: '/mail-resend/' }
        ]
    },
    {
        text: 'Utils',
        link: '/utils/',
        activeMatch: '^/utils/',
    }
];

export const sidebar = {
    "/guide/": guide,
    "/asp-net-core/": aspNetCore,
    "/asp-net-auth-credentials/": aspNetAuthCredentials,
    "/asp-net-auth/": aspNetAuth,
    "/asp-net-localization/": aspNetLocalization,
    "/asp-net-maintenance-mode/": aspNetMaintenanceMode,
    "/asp-net-request-validation/": aspNetRequestValidation,
    "/console-commands/": consoleCommands,
    "/utils/": utils,
    "/ef-core-model-building/": efCoreModelBuilding,
    "/hangfire-recurring-jobs/": hangfireRecurringJobs,
    "/hosting-console-lifetime/": hostingConsoleLifetime,
    "/mail-resend/": mailResend,
    "/remote-commands/": remoteCommands,
    "/serilog/": serilog,
};
