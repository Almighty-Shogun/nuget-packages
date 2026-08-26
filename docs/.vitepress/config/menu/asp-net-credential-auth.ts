import type { DefaultTheme } from 'vitepress'

export const aspNetCredentialAuth: DefaultTheme.SidebarItem[] = [
    {
        text: 'ASP.NET Credential Auth',
        items: [
            { text: 'Introduction', link: '/asp-net-credential-auth/' },
            { text: 'Installation', link: '/asp-net-credential-auth/installation' },
            { text: 'Configuration', link: '/asp-net-credential-auth/configuration' },
            { text: 'Exceptions', link: '/asp-net-credential-auth/exceptions' },
            { text: 'Localization', link: '/asp-net-credential-auth/localization' }
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
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'AuthPasswordService', link: '/asp-net-credential-auth/services/auth-password-service' },
            { text: 'AuthSessionService', link: '/asp-net-credential-auth/services/auth-session-service' },
            { text: 'AuthTokenService', link: '/asp-net-credential-auth/services/auth-token-service' },
            { text: 'AuthTwoFactorService', link: '/asp-net-credential-auth/services/auth-two-factor-service' },
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
            { text: 'Login', link: '/asp-net-credential-auth/requests/login-request' },
            { text: 'Register', link: '/asp-net-credential-auth/requests/register-request' }
        ]
    },
    {
        text: 'Results',
        collapsed: false,
        items: [
            { text: 'AuthSessionResult', link: '/asp-net-credential-auth/results/auth-session-result' },
            { text: 'AuthTwoFactorResult', link: '/asp-net-credential-auth/results/auth-two-factor-result' }
        ]
    },
    {
        text: 'Utilities',
        collapsed: false,
        items: [
            { text: 'TokenHasher', link: '/asp-net-credential-auth/utilities/token-hasher' }
        ]
    },
    {
        text: 'Types',
        collapsed: false,
        items: [
            { text: 'AuthDbContext', link: '/asp-net-credential-auth/types/auth-db-context' },
            { text: 'AuthUser', link: '/asp-net-credential-auth/types/auth-user' },
            { text: 'EmailVerificationToken', link: '/asp-net-credential-auth/types/email-verification-token' },
            { text: 'PasswordResetToken', link: '/asp-net-credential-auth/types/password-reset-token' },
            { text: 'TwoFactorRecoveryCode', link: '/asp-net-credential-auth/types/two-factor-recovery-code' },
            { text: 'UserLockout', link: '/asp-net-credential-auth/types/user-lockout' },
            { text: 'UserSession', link: '/asp-net-credential-auth/types/user-session' },
            { text: 'UserTwoFactor', link: '/asp-net-credential-auth/types/user-two-factor' }
        ]
    },
];
