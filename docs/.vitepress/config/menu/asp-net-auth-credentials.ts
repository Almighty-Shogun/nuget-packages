import type { DefaultTheme } from 'vitepress'

export const aspNetAuthCredentials: DefaultTheme.SidebarItem[] = [
    {
        text: 'ASP.NET Auth Credentials',
        items: [
            { text: 'Introduction', link: '/asp-net-auth-credentials/' },
            { text: 'Installation', link: '/asp-net-auth-credentials/installation' },
            { text: 'Configuration', link: '/asp-net-auth-credentials/configuration' },
            { text: 'Exceptions', link: '/asp-net-auth-credentials/exceptions' },
            { text: 'Localization', link: '/asp-net-auth-credentials/localization' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddAuthCredentials', link: '/asp-net-auth-credentials/extensions/add-auth-credentials' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'AuthPasswordService', link: '/asp-net-auth-credentials/services/auth-password-service' },
            { text: 'AuthSessionService', link: '/asp-net-auth-credentials/services/auth-session-service' },
            { text: 'AuthTwoFactorService', link: '/asp-net-auth-credentials/services/auth-two-factor-service' },
            { text: 'AuthUserService', link: '/asp-net-auth-credentials/services/auth-user-service' }
        ]
    },
    {
        text: 'Requests',
        collapsed: false,
        items: [
            { text: 'ChangePassword', link: '/asp-net-auth-credentials/requests/change-password-request' },
            { text: 'CompleteForgotPassword', link: '/asp-net-auth-credentials/requests/complete-forgot-password-request' },
            { text: 'CreateUser', link: '/asp-net-auth-credentials/requests/create-user-request' },
            { text: 'ForgotPassword', link: '/asp-net-auth-credentials/requests/forgot-password-request' },
            { text: 'Login', link: '/asp-net-auth-credentials/requests/login-request' },
            { text: 'Register', link: '/asp-net-auth-credentials/requests/register-request' }
        ]
    },
    {
        text: 'Results',
        collapsed: false,
        items: [
            { text: 'AuthSessionResult', link: '/asp-net-auth-credentials/results/auth-session-result' },
            { text: 'AuthTwoFactorResult', link: '/asp-net-auth-credentials/results/auth-two-factor-result' }
        ]
    },
    {
        text: 'Utilities',
        collapsed: false,
        items: [
            { text: 'TokenHasher', link: '/asp-net-auth-credentials/utilities/token-hasher' }
        ]
    },
    {
        text: 'Types',
        collapsed: false,
        items: [
            { text: 'AuthDbContext', link: '/asp-net-auth-credentials/types/auth-db-context' },
            { text: 'AuthUser', link: '/asp-net-auth-credentials/types/auth-user' },
            { text: 'EmailVerificationToken', link: '/asp-net-auth-credentials/types/email-verification-token' },
            { text: 'PasswordResetToken', link: '/asp-net-auth-credentials/types/password-reset-token' },
            { text: 'TwoFactorRecoveryCode', link: '/asp-net-auth-credentials/types/two-factor-recovery-code' },
            { text: 'UserLockout', link: '/asp-net-auth-credentials/types/user-lockout' },
            { text: 'UserSession', link: '/asp-net-auth-credentials/types/user-session' },
            { text: 'UserTwoFactor', link: '/asp-net-auth-credentials/types/user-two-factor' }
        ]
    },
];
