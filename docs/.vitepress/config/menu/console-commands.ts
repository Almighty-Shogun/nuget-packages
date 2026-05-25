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
        text: 'Attributes',
        collapsed: false,
        items: [
            { text: 'AliasAttribute', link: '/console-commands/attributes/alias-attribute' },
            { text: 'ConsoleCommandAttribute', link: '/console-commands/attributes/console-command-attribute' },
            { text: 'ExampleAttribute', link: '/console-commands/attributes/example-attribute' }
        ]
    },
    {
        text: 'Classes',
        collapsed: false,
        items: [
            { text: 'ConsoleCommand', link: '/console-commands/classes/console-command' },
            { text: 'ConsoleCommandBase', link: '/console-commands/classes/console-command-base/' },
            {
                text: 'ConsoleCommandHandler',
                link: '/console-commands/classes/console-command-handler/',
                collapsed: true,
                items: [
                    { text: 'StartAsync', link: '/console-commands/classes/console-command-handler/start-async' }
                ]
            },
            {
                text: 'ConsoleUtils',
                link: '/console-commands/classes/console-utils/',
                collapsed: true,
                items: [
                    { text: 'AskQuestionAsync', link: '/console-commands/classes/console-utils/ask-question-async' },
                    { text: 'GetAllCommands', link: '/console-commands/classes/console-utils/get-all-commands' },
                    { text: 'RemoveLastConsoleLine', link: '/console-commands/classes/console-utils/remove-last-console-line' }
                ]
            }
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
];
