import type { DefaultTheme } from 'vitepress'

export const consoleCommands: DefaultTheme.SidebarItem[] = [
    {
        text: 'Console Commands',
        items: [
            { text: 'Introduction', link: '/console-commands/' },
            { text: 'Installation', link: '/console-commands/installation' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddConsoleCommands', link: '/console-commands/extensions/add-console-commands' },
            { text: 'RegisterConsoleCommands', link: '/console-commands/extensions/register-console-commands' }
        ]
    },
    {
        text: 'Attributes',
        collapsed: false,
        items: [
            { text: 'Alias', link: '/console-commands/attributes/alias-attribute' },
            { text: 'ConsoleCommand', link: '/console-commands/attributes/console-command-attribute' },
            { text: 'Example', link: '/console-commands/attributes/example-attribute' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'ConsoleCommandHandler', link: '/console-commands/services/console-command-handler' }
        ]
    },
    {
        text: 'Types',
        collapsed: false,
        items: [
            { text: 'ConsoleCommand', link: '/console-commands/types/console-command' },
            { text: 'ConsoleCommandBase', link: '/console-commands/types/console-command-base' },
            { text: 'ConsoleUtils', link: '/console-commands/types/console-utils' }
        ]
    },
];
