import type { DefaultTheme } from 'vitepress'

export const aspNetCredentialAuth: DefaultTheme.SidebarItem[] = [
    {
        text: 'ASP.NET Credential Auth',
        items: [
            { text: 'Introduction', link: '/asp-net-credential-auth/' },
            { text: 'Installation', link: '/asp-net-credential-auth/installation' },
            { text: 'Localization', link: '/asp-net-credential-auth/localization' },
            { text: 'Controllers', link: '/asp-net-credential-auth/controllers' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddCredentialAuth', link: '/asp-net-credential-auth/extensions/add-credential-auth' }
        ]
    },
    {
        text: 'Attributes',
        collapsed: false,
        items: [
            { text: 'CurrentPassword', link: '/asp-net-credential-auth/attributes/current-password-attribute' },
            { text: 'LoginIdentifierExists', link: '/asp-net-credential-auth/attributes/login-identifier-exists-attribute' },
            { text: 'NotCurrentPassword', link: '/asp-net-credential-auth/attributes/not-current-password-attribute' },
            { text: 'PasswordMatch', link: '/asp-net-credential-auth/attributes/password-match-attribute' },
            { text: 'PasswordResetToken', link: '/asp-net-credential-auth/attributes/password-reset-token-attribute' },
            { text: 'UniqueEmail', link: '/asp-net-credential-auth/attributes/unique-email-attribute' },
            { text: 'UniqueUsername', link: '/asp-net-credential-auth/attributes/unique-username-attribute' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'ApplicationAuthService', link: '/asp-net-credential-auth/services/application-auth-service' },
            { text: 'AuthPasswordService', link: '/asp-net-credential-auth/services/auth-password-service' },
            { text: 'AuthSessionService', link: '/asp-net-credential-auth/services/auth-session-service' },
            { text: 'AuthTokenService', link: '/asp-net-credential-auth/services/auth-token-service' },
            { text: 'AuthUserService', link: '/asp-net-credential-auth/services/auth-user-service' }
        ]
    },
    {
        text: 'Requests',
        collapsed: false,
        items: [
            { text: 'ChangePassword', link: '/asp-net-credential-auth/requests/change-password-request' },
            { text: 'CompleteForgotPassword', link: '/asp-net-credential-auth/requests/complete-forgot-password-request' },
            { text: 'CreateUser', link: '/asp-net-credential-auth/requests/create-user-request' },
            { text: 'ForgotPassword', link: '/asp-net-credential-auth/requests/forgot-password-request' },
            { text: 'ForgotPasswordToken', link: '/asp-net-credential-auth/requests/forgot-password-token-request' },
            { text: 'Login', link: '/asp-net-credential-auth/requests/login-request' },
            { text: 'Register', link: '/asp-net-credential-auth/requests/register-request' }
        ]
    },
    {
        text: 'Results',
        collapsed: false,
        items: [
            { text: 'AuthSessionResult', link: '/asp-net-credential-auth/results/auth-session-result' }
        ]
    },
    {
        text: 'Types',
        collapsed: false,
        items: [
            { text: 'AuthDbContext', link: '/asp-net-credential-auth/types/auth-db-context' },
            { text: 'AuthUser', link: '/asp-net-credential-auth/types/auth-user' },
            { text: 'PasswordResetToken', link: '/asp-net-credential-auth/types/password-reset-token' },
            { text: 'UserSession', link: '/asp-net-credential-auth/types/user-session' }
        ]
    },
];
