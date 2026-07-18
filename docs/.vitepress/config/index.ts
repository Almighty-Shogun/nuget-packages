import { defineConfig } from 'vitepress'
import { nav, sidebar } from './menu'
import { groupIconMdPlugin, groupIconVitePlugin } from 'vitepress-plugin-group-icons'

export default defineConfig({
    title: 'NuGet Packages',
    titleTemplate: ':title — NuGet Packages',
    description: 'Documentation for all C#/NuGet packages in the AlmightyShogun.* monorepo.',
    cleanUrls: true,
    lastUpdated: true,
    srcExclude: ['guide-node/**', 'AGENTS.md'],
    sitemap: {
        hostname: 'https://nuget-packages.shogun.ms'
    },
    head: [
        ['link', { rel: 'icon', type: 'image/svg+xml', href: '/favicon.svg' }],
        ['link', { rel: 'stylesheet', href: 'https://font.shogun.ms/css?family=inter-variable|jetbrains-mono' }],
        ['meta', { property: 'og:type', content: 'website' }],
        ['meta', { property: 'og:url', content: 'https://nuget-packages.shogun.ms/' }]
    ],
    markdown: {
        config(md) {
            md.use(groupIconMdPlugin)
        }
    },
    vite: {
        plugins: [
            groupIconVitePlugin() as any
        ]
    },
    themeConfig: {
        logo: '/logo.svg',
        nav,
        sidebar,
        search: {
            provider: 'local'
        },
        socialLinks: [
            { icon: 'github', link: 'https://github.com/Almighty-Shogun/nuget-packages' },
            { icon: 'discord', link: 'https://discord.gg/QJKU4kdyep' }
        ],
        footer: {
            message: 'All packages are released under the <a href="https://github.com/Almighty-Shogun/nuget-packages/blob/main/LICENSE">MIT</a> License.',
            copyright: 'Copyright © 2025—present <a href="https://github.com/Almighty-Shogun">Almighty Shogun</a>'
        },
        editLink: {
            pattern: 'https://github.com/Almighty-Shogun/nuget-packages/edit/main/docs/:path',
            text: 'Edit this page on GitHub'
        }
    }
})
