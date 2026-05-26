import type { DefaultTheme } from 'vitepress'

export const resendUtils: DefaultTheme.SidebarItem[] = [
    {
        text: 'Resend Utils',
        items: [
            { text: 'Introduction', link: '/resend-utils/' },
            { text: 'Installation', link: '/resend-utils/installation' },
            { text: 'Configuration', link: '/resend-utils/configuration' }
        ]
    },
    {
        text: 'Classes',
        collapsed: false,
        items: [
            {
                text: 'BaseMailTemplate',
                link: '/resend-utils/classes/base-mail-template/'
            }
        ]
    },
    {
        text: 'Configuration',
        collapsed: false,
        items: [
            { text: 'EmailSettings', link: '/resend-utils/configuration/email-settings/' },
            { text: 'EmailTemplateSettings', link: '/resend-utils/configuration/email-template-settings/' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddResendEmail', link: '/resend-utils/extensions/add-resend-email' }
        ]
    },
    {
        text: 'Interfaces',
        collapsed: false,
        items: [
            {
                text: 'IResendMailService',
                link: '/resend-utils/interfaces/iresend-mail-service/',
                collapsed: true,
                items: [
                    { text: 'SendAsync', link: '/resend-utils/interfaces/iresend-mail-service/send-async' }
                ]
            }
        ]
    },
    {
        text: 'Records',
        collapsed: false,
        items: [
            { text: 'MailButton', link: '/resend-utils/records/mail-button' }
        ]
    },
];
