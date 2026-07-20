import type { DefaultTheme } from 'vitepress'
import { guide } from './guide'
import { aspNetCredentialAuth } from './asp-net-credential-auth'
import { aspNetJwtAuth } from './asp-net-jwt-auth'
import { aspNetMaintenance } from './asp-net-maintenance'
import { aspNetUtils } from './asp-net-utils'
import { aspNetValidation } from './asp-net-validation'
import { consoleCommands } from './console-commands'
import { efCoreUtils } from './ef-core-utils'
import { hangfireUtils } from './hangfire-utils'
import { hostingUtils } from './hosting-utils'
import { logging } from './logging'
import { remoteCommands } from './remote-commands'
import { resendUtils } from './resend-utils'
import { utils } from './utils'

export const nav: DefaultTheme.NavItem[] = [
    {
        text: 'Guide',
        link: '/guide/',
        activeMatch: '^/guide/',
    },
    {
        text: 'ASP.NET',
        activeMatch: '^/(asp-net-credential-auth|asp-net-jwt-auth|asp-net-maintenance|asp-net-utils|asp-net-validation)/',
        items: [
            { text: 'JWT Auth', activeMatch: '^/asp-net-jwt-auth/', link: '/asp-net-jwt-auth/' },
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
        activeMatch: '^/(console-commands|hangfire-utils|hosting-utils|logging|remote-commands)/',
        items: [
            { text: 'Console Commands', activeMatch: '^/console-commands/', link: '/console-commands/' },
            { text: 'Hangfire Utils', activeMatch: '^/hangfire-utils/', link: '/hangfire-utils/' },
            { text: 'Hosting Utils', activeMatch: '^/hosting-utils/', link: '/hosting-utils/' },
            { text: 'Logging', activeMatch: '^/logging/', link: '/logging/' },
            { text: 'Remote Commands', activeMatch: '^/remote-commands/', link: '/remote-commands/' }
        ]
    },
    {
        text: 'Data',
        activeMatch: '^/(ef-core-utils|resend-utils)/',
        items: [
            { text: 'EF Core Utils', activeMatch: '^/ef-core-utils/', link: '/ef-core-utils/' },
            { text: 'Resend Utils', activeMatch: '^/resend-utils/', link: '/resend-utils/' }
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
    "/asp-net-credential-auth/": aspNetCredentialAuth,
    "/asp-net-jwt-auth/": aspNetJwtAuth,
    "/asp-net-maintenance/": aspNetMaintenance,
    "/asp-net-utils/": aspNetUtils,
    "/asp-net-validation/": aspNetValidation,
    "/console-commands/": consoleCommands,
    "/ef-core-utils/": efCoreUtils,
    "/hangfire-utils/": hangfireUtils,
    "/hosting-utils/": hostingUtils,
    "/logging/": logging,
    "/remote-commands/": remoteCommands,
    "/resend-utils/": resendUtils,
    "/utils/": utils,
};
