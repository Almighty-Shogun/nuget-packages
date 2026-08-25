import type { DefaultTheme } from 'vitepress'

export const remoteCommands: DefaultTheme.SidebarItem[] = [
    {
        text: 'Remote Commands',
        items: [
            { text: 'Introduction', link: '/remote-commands/' },
            { text: 'Installation', link: '/remote-commands/installation' },
            { text: 'Configuration', link: '/remote-commands/configuration' },
            { text: 'Exceptions', link: '/remote-commands/exceptions' }
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
            { text: 'CommandResponse', link: '/remote-commands/services/command-response' },
            { text: 'RemoteCommandClient', link: '/remote-commands/services/remote-command-client' },
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
    {
        text: 'Records',
        collapsed: false,
        items: [
            { text: 'RemoteCommandPayload', link: '/remote-commands/records/remote-command-payload' },
            { text: 'RemoteCommandResponse', link: '/remote-commands/records/remote-command-response' }
        ]
    },
];
