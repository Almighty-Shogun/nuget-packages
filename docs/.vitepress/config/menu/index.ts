import type { DefaultTheme } from 'vitepress'
import { guide } from './guide'
import { aspNetJwtAuth } from './asp-net-jwt-auth'
import { aspNetUtils } from './asp-net-utils'
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
        text: 'Packages',
        activeMatch: '^/(asp-net-jwt-auth|asp-net-utils|console-commands|ef-core-utils|hangfire-utils|hosting-utils|logging|remote-commands|resend-utils|utils)/',
        items: [
            { text: 'ASP.NET JWT Auth', activeMatch: '^/asp-net-jwt-auth/', link: '/asp-net-jwt-auth/' },
            { text: 'ASP.NET Utils', activeMatch: '^/asp-net-utils/', link: '/asp-net-utils/' },
            { text: 'Console Commands', activeMatch: '^/console-commands/', link: '/console-commands/' },
            { text: 'Entity Framework Core Utils', activeMatch: '^/ef-core-utils/', link: '/ef-core-utils/' },
            { text: 'Hangfire Utils', activeMatch: '^/hangfire-utils/', link: '/hangfire-utils/' },
            { text: 'Hosting Utils', activeMatch: '^/hosting-utils/', link: '/hosting-utils/' },
            { text: 'Logging', activeMatch: '^/logging/', link: '/logging/' },
            { text: 'Remote Commands', activeMatch: '^/remote-commands/', link: '/remote-commands/' },
            { text: 'Resend Utils', activeMatch: '^/resend-utils/', link: '/resend-utils/' },
            { text: 'Utils', activeMatch: '^/utils/', link: '/utils/' }
        ]
    }
];

export const sidebar = {
    "/guide/": guide,
    "/asp-net-jwt-auth/": aspNetJwtAuth,
    "/asp-net-utils/": aspNetUtils,
    "/console-commands/": consoleCommands,
    "/ef-core-utils/": efCoreUtils,
    "/hangfire-utils/": hangfireUtils,
    "/hosting-utils/": hostingUtils,
    "/logging/": logging,
    "/remote-commands/": remoteCommands,
    "/resend-utils/": resendUtils,
    "/utils/": utils,
};
