import type { DefaultTheme } from 'vitepress'

export const aspNetLocalization: DefaultTheme.SidebarItem[] = [
    {
        text: 'AspNet Localization',
        items: [
            { text: 'Introduction', link: '/asp-net-localization/' },
            { text: 'Installation', link: '/asp-net-localization/installation' },
            { text: 'Configuration', link: '/asp-net-localization/configuration' },
            { text: 'Localization', link: '/asp-net-localization/localization' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddMessageLocalization', link: '/asp-net-localization/extensions/add-message-localization' },
            { text: 'GetAcceptLanguage', link: '/asp-net-localization/extensions/get-accept-language' },
            { text: 'GetAcceptLanguages', link: '/asp-net-localization/extensions/get-accept-languages' },
            { text: 'GetContentLanguage', link: '/asp-net-localization/extensions/get-content-language' },
            { text: 'TrySetContentLanguage', link: '/asp-net-localization/extensions/try-set-content-language' },
            { text: 'UseMessageLocalization', link: '/asp-net-localization/extensions/use-message-localization' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'LanguageProvider', link: '/asp-net-localization/services/language-provider' },
            { text: 'MessageResolver', link: '/asp-net-localization/services/message-resolver' }
        ]
    },
];
