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
        text: 'Configuration',
        collapsed: false,
        items: [
            { text: 'RemoteServerSettings', link: '/remote-commands/configuration/remote-server-settings' }
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
    {
        text: 'Attributes',
        collapsed: false,
        items: [
            { text: 'RemoteCommand', link: '/remote-commands/attributes/remote-command-attribute' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'RemoteCommandHandler', link: '/remote-commands/services/remote-command-handler' }
        ]
    },
    {
        text: 'Types',
        collapsed: false,
        items: [
            { text: 'RemoteCommand<T>', link: '/remote-commands/types/remote-command' }
        ]
    },
];
