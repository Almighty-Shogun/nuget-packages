import type { DefaultTheme } from 'vitepress'

export const remoteCommands: DefaultTheme.SidebarItem[] = [
    {
        text: 'Remote Commands',
        items: [
            { text: 'Introduction', link: '/remote-commands/' },
            { text: 'Installation', link: '/remote-commands/installation' },
            { text: 'Configuration', link: '/remote-commands/configuration' }
        ]
    },
    {
        text: 'Attributes',
        collapsed: false,
        items: [
            { text: 'RemoteCommandAttribute', link: '/remote-commands/attributes/remote-command-attribute' }
        ]
    },
    {
        text: 'Classes',
        collapsed: false,
        items: [
            {
                text: 'RemoteCommand<T>',
                link: '/remote-commands/classes/remote-command/',
                collapsed: true,
                items: [
                    { text: 'HandleCommandAsync', link: '/remote-commands/classes/remote-command/handle-command-async' },
                    { text: 'WriteResponseAsync', link: '/remote-commands/classes/remote-command/write-response-async' }
                ]
            },
            {
                text: 'RemoteCommandHandler',
                link: '/remote-commands/classes/remote-command-handler/',
                collapsed: true,
                items: [
                    { text: 'StartAsync', link: '/remote-commands/classes/remote-command-handler/start-async' },
                    { text: 'Stop', link: '/remote-commands/classes/remote-command-handler/stop' }
                ]
            }
        ]
    },
    {
        text: 'Configuration',
        collapsed: false,
        items: [
            { text: 'RemoteServerSettings', link: '/remote-commands/configuration/remote-server-settings/' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddRemoteCommands', link: '/remote-commands/extensions/add-remote-commands' },
            { text: 'RegisterRemoteCommands', link: '/remote-commands/extensions/register-remote-commands' }
        ]
    },
];
