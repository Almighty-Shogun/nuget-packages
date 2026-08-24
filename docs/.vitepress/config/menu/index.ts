import type { DefaultTheme } from 'vitepress'
import { guide } from './guide'
import { aspNetCredentialAuth } from './asp-net-credential-auth'
import { aspNetJwtAuth } from './asp-net-jwt-auth'
import { aspNetLocalization } from './asp-net-localization'
import { aspNetMaintenance } from './asp-net-maintenance'
import { aspNetUtils } from './asp-net-utils'
import { aspNetValidation } from './asp-net-validation'
import { consoleCommands } from './console-commands'
import { core } from './core'
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
        activeMatch: '^/(asp-net-credential-auth|asp-net-jwt-auth|asp-net-localization|asp-net-maintenance|asp-net-utils|asp-net-validation)/',
        items: [
            { text: 'JWT Auth', activeMatch: '^/asp-net-jwt-auth/', link: '/asp-net-jwt-auth/' },
            { text: 'Localization', activeMatch: '^/asp-net-localization/', link: '/asp-net-localization/' },
            { text: 'Maintenance', activeMatch: '^/asp-net-maintenance/', link: '/asp-net-maintenance/' },
            { text: 'Utils', activeMatch: '^/asp-net-utils/', link: '/asp-net-utils/' },
            { text: 'Validation', activeMatch: '^/asp-net-validation/', link: '/asp-net-validation/' },
            {
                text: 'Login Systems',
                items: [
                    { text: 'Credential Auth', activeMatch: '^/asp-net-credential-auth/', link: '/asp-net-credential-auth/' }
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
        text: 'Core',
        link: '/core/',
        activeMatch: '^/core/',
    }
];

export const sidebar = {
    "/guide/": guide,
    "/asp-net-credential-auth/": aspNetCredentialAuth,
    "/asp-net-jwt-auth/": aspNetJwtAuth,
    "/asp-net-localization/": aspNetLocalization,
    "/asp-net-maintenance/": aspNetMaintenance,
    "/asp-net-utils/": aspNetUtils,
    "/asp-net-validation/": aspNetValidation,
    "/console-commands/": consoleCommands,
    "/core/": core,
    "/ef-core-model-building/": efCoreModelBuilding,
    "/hangfire-recurring-jobs/": hangfireRecurringJobs,
    "/hosting-console-lifetime/": hostingConsoleLifetime,
    "/mail-resend/": mailResend,
    "/remote-commands/": remoteCommands,
    "/serilog/": serilog,
};
