import type { DefaultTheme } from 'vitepress'

export const mailResend: DefaultTheme.SidebarItem[] = [
    {
        text: 'Mail Resend',
        items: [
            { text: 'Introduction', link: '/mail-resend/' },
            { text: 'Installation', link: '/mail-resend/installation' },
            { text: 'Configuration', link: '/mail-resend/configuration' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddResendEmail', link: '/mail-resend/extensions/add-resend-email' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'ResendMailService', link: '/mail-resend/services/resend-mail-service' }
        ]
    },
    {
        text: 'Types',
        collapsed: false,
        items: [
            { text: 'BaseMailTemplate', link: '/mail-resend/types/base-mail-template' }
        ]
    },
    {
        text: 'Records',
        collapsed: false,
        items: [
            { text: 'MailAttachment', link: '/mail-resend/records/mail-attachment' },
            { text: 'MailButton', link: '/mail-resend/records/mail-button' },
            { text: 'MailOptions', link: '/mail-resend/records/mail-options' },
            { text: 'MailPreview', link: '/mail-resend/records/mail-preview' },
            { text: 'MailSendResult', link: '/mail-resend/records/mail-send-result' }
        ]
    },
];
