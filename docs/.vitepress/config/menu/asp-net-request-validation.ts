import type { DefaultTheme } from 'vitepress'

export const aspNetRequestValidation: DefaultTheme.SidebarItem[] = [
    {
        text: 'ASP.NET Request Validation',
        items: [
            { text: 'Introduction', link: '/asp-net-request-validation/' },
            { text: 'Installation', link: '/asp-net-request-validation/installation' },
            { text: 'Localization', link: '/asp-net-request-validation/localization' },
            { text: 'Fluent Validation', link: '/asp-net-request-validation/fluent-validation' },
            { text: 'Custom Rules', link: '/asp-net-request-validation/custom-rules' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddAspNetValidation', link: '/asp-net-request-validation/extensions/add-asp-net-validation' },
            { text: 'UseAspNetValidation', link: '/asp-net-request-validation/extensions/use-asp-net-validation' }
        ]
    },
    {
        text: 'Validation Rules',
        collapsed: false,
        items: [
            { text: 'Presence', link: '/asp-net-request-validation/validation-rules/presence' },
            { text: 'Conditional Presence', link: '/asp-net-request-validation/validation-rules/conditional-presence' },
            { text: 'Composition', link: '/asp-net-request-validation/validation-rules/composition' },
            { text: 'Comparison', link: '/asp-net-request-validation/validation-rules/comparison' },
            { text: 'Strings', link: '/asp-net-request-validation/validation-rules/strings' },
            { text: 'Formats', link: '/asp-net-request-validation/validation-rules/formats' },
            { text: 'Numbers', link: '/asp-net-request-validation/validation-rules/numbers' },
            { text: 'Passwords', link: '/asp-net-request-validation/validation-rules/passwords' },
            { text: 'Dates', link: '/asp-net-request-validation/validation-rules/dates' },
            { text: 'Types and Files', link: '/asp-net-request-validation/validation-rules/types-and-files' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'ValidationRuleDescriber', link: '/asp-net-request-validation/services/validation-rule-describer' }
        ]
    },
    {
        text: 'Utilities',
        collapsed: false,
        items: [
            { text: 'ValidationErrorResult', link: '/asp-net-request-validation/utilities/validation-error-result' }
        ]
    },
    {
        text: 'Types',
        collapsed: false,
        items: [
            { text: 'ComparisonTarget', link: '/asp-net-request-validation/types/comparison-target' }
        ]
    },
    {
        text: 'Records',
        collapsed: false,
        items: [
            { text: 'ValidationErrorResponse', link: '/asp-net-request-validation/records/validation-error-response' },
            { text: 'ValidationRuleDescription', link: '/asp-net-request-validation/records/validation-rule-description' },
            { text: 'ValidationRuleError', link: '/asp-net-request-validation/records/validation-rule-error' },
            { text: 'ValidationRuleResult', link: '/asp-net-request-validation/records/validation-rule-result' }
        ]
    },
];
