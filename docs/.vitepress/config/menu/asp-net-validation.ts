import type { DefaultTheme } from 'vitepress'

export const aspNetValidation: DefaultTheme.SidebarItem[] = [
    {
        text: 'ASP.NET Validation',
        items: [
            { text: 'Introduction', link: '/asp-net-validation/' },
            { text: 'Installation', link: '/asp-net-validation/installation' },
            { text: 'Localization', link: '/asp-net-validation/localization' },
            { text: 'Fluent Validation', link: '/asp-net-validation/fluent-validation' },
            { text: 'Custom Rules', link: '/asp-net-validation/custom-rules' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddAspNetValidation', link: '/asp-net-validation/extensions/add-asp-net-validation' },
            { text: 'UseAspNetValidation', link: '/asp-net-validation/extensions/use-asp-net-validation' }
        ]
    },
    {
        text: 'Validation Rules',
        collapsed: false,
        items: [
            { text: 'Presence', link: '/asp-net-validation/validation-rules/presence' },
            { text: 'Conditional Presence', link: '/asp-net-validation/validation-rules/conditional-presence' },
            { text: 'Composition', link: '/asp-net-validation/validation-rules/composition' },
            { text: 'Comparison', link: '/asp-net-validation/validation-rules/comparison' },
            { text: 'Strings', link: '/asp-net-validation/validation-rules/strings' },
            { text: 'Formats', link: '/asp-net-validation/validation-rules/formats' },
            { text: 'Numbers', link: '/asp-net-validation/validation-rules/numbers' },
            { text: 'Passwords', link: '/asp-net-validation/validation-rules/passwords' },
            { text: 'Dates', link: '/asp-net-validation/validation-rules/dates' },
            { text: 'Types and Files', link: '/asp-net-validation/validation-rules/types-and-files' }
        ]
    },
    {
        text: 'Types',
        collapsed: false,
        items: [
            { text: 'ComparisonTarget', link: '/asp-net-validation/types/comparison-target' },
            { text: 'ValidationErrorResult', link: '/asp-net-validation/types/validation-error-result' },
            { text: 'ValidationException', link: '/asp-net-validation/types/validation-exception' }
        ]
    },
    {
        text: 'Records',
        collapsed: false,
        items: [
            { text: 'ValidationErrorResponse', link: '/asp-net-validation/records/validation-error-response' },
            { text: 'ValidationRuleError', link: '/asp-net-validation/records/validation-rule-error' },
            { text: 'ValidationRuleResult', link: '/asp-net-validation/records/validation-rule-result' }
        ]
    },
];
