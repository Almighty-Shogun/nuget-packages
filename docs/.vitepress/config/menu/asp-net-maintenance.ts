import type { DefaultTheme } from 'vitepress'

export const aspNetMaintenance: DefaultTheme.SidebarItem[] = [
    {
        text: 'ASP.NET Maintenance',
        items: [
            { text: 'Introduction', link: '/asp-net-maintenance/' },
            { text: 'Installation', link: '/asp-net-maintenance/installation' },
            { text: 'Configuration', link: '/asp-net-maintenance/configuration' },
            { text: 'Maintenance Controls', link: '/asp-net-maintenance/maintenance-controls' }
        ]
    },
    {
        text: 'Configuration',
        collapsed: false,
        items: [
            { text: 'MaintenanceSettings', link: '/asp-net-maintenance/configuration/maintenance-settings' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddMaintenanceMode', link: '/asp-net-maintenance/extensions/add-maintenance-mode' },
            { text: 'UseMaintenanceMode', link: '/asp-net-maintenance/extensions/use-maintenance-mode' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'MaintenanceService', link: '/asp-net-maintenance/services/maintenance-service' }
        ]
    },
    {
        text: 'Types',
        collapsed: false,
        items: [
            { text: 'MaintenanceRequest', link: '/asp-net-maintenance/types/maintenance-request' },
            { text: 'MaintenanceState', link: '/asp-net-maintenance/types/maintenance-state' }
        ]
    },
];
