import type { DefaultTheme } from 'vitepress'

export const aspNetMaintenanceMode: DefaultTheme.SidebarItem[] = [
    {
        text: 'ASP.NET Maintenance Mode',
        items: [
            { text: 'Introduction', link: '/asp-net-maintenance-mode/' },
            { text: 'Installation', link: '/asp-net-maintenance-mode/installation' },
            { text: 'Configuration', link: '/asp-net-maintenance-mode/configuration' }
        ]
    },
    {
        text: 'Extensions',
        collapsed: false,
        items: [
            { text: 'AddMaintenanceMode', link: '/asp-net-maintenance-mode/extensions/add-maintenance-mode' },
            { text: 'UseMaintenanceMode', link: '/asp-net-maintenance-mode/extensions/use-maintenance-mode' }
        ]
    },
    {
        text: 'Services',
        collapsed: false,
        items: [
            { text: 'MaintenanceService', link: '/asp-net-maintenance-mode/services/maintenance-service' }
        ]
    },
    {
        text: 'Types',
        collapsed: false,
        items: [
            { text: 'MaintenanceRequest', link: '/asp-net-maintenance-mode/types/maintenance-request' },
            { text: 'MaintenanceResponse', link: '/asp-net-maintenance-mode/types/maintenance-response' },
            { text: 'MaintenanceState', link: '/asp-net-maintenance-mode/types/maintenance-state' }
        ]
    },
];
