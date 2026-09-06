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
        text: 'Requests',
        collapsed: false,
        items: [
            { text: 'ChangePassword', link: '/asp-net-auth-credentials/requests/change-password-request' },
            { text: 'CompleteEmailVerification', link: '/asp-net-auth-credentials/requests/complete-email-verification-request' },
            { text: 'CompleteForgotPassword', link: '/asp-net-auth-credentials/requests/complete-forgot-password-request' },
            { text: 'CompleteTwoFactorLogin', link: '/asp-net-auth-credentials/requests/complete-two-factor-login-request' },
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
            { text: 'AuthLoginResult', link: '/asp-net-auth-credentials/results/auth-login-result' },
            { text: 'AuthSessionResult', link: '/asp-net-auth-credentials/results/auth-session-result' },
            { text: 'AuthTwoFactorResult', link: '/asp-net-auth-credentials/results/auth-two-factor-result' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'AuthEmailService', link: '/asp-net-auth-credentials/services/auth-email-service' },
            { text: 'AuthPasswordService', link: '/asp-net-auth-credentials/services/auth-password-service' },
            { text: 'AuthSessionService', link: '/asp-net-auth-credentials/services/auth-session-service' },
            { text: 'AuthTwoFactorService', link: '/asp-net-auth-credentials/services/auth-two-factor-service' },
            { text: 'AuthUserService', link: '/asp-net-auth-credentials/services/auth-user-service' }
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
            { text: 'EmailVerificationPurpose', link: '/asp-net-auth-credentials/types/email-verification-purpose' },
            { text: 'EmailVerificationToken', link: '/asp-net-auth-credentials/types/email-verification-token' },
            { text: 'PasswordResetToken', link: '/asp-net-auth-credentials/types/password-reset-token' },
            { text: 'TwoFactorChallenge', link: '/asp-net-auth-credentials/types/two-factor-challenge' },
            { text: 'TwoFactorRecoveryCode', link: '/asp-net-auth-credentials/types/two-factor-recovery-code' },
            { text: 'UserLockout', link: '/asp-net-auth-credentials/types/user-lockout' },
            { text: 'UserSession', link: '/asp-net-auth-credentials/types/user-session' },
            { text: 'UserTwoFactor', link: '/asp-net-auth-credentials/types/user-two-factor' }
        ]
    },
];
