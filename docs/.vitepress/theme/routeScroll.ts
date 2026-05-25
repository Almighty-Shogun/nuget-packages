import type { Router } from 'vitepress'
import { scrollToTop } from '@almighty-shogun/utils'

export function registerRouteScroll(router: Router): void {
    if (typeof window === 'undefined') return;

    let currentPackage = getPackageSection(router.route.path);
    const previousAfterRouteChange = router.onAfterRouteChange;

    router.onAfterRouteChange = async (to) => {
        await previousAfterRouteChange?.(to);

        const nextPackage = getPackageSection(to);
        const packageChanged = nextPackage !== currentPackage;

        currentPackage = nextPackage;

        afterRouteRendered(() => {
            scrollToTop();
            scrollSidebar(to, packageChanged);
        });
    };
}

function getPackageSection(path: string): string {
    return path.split('/').filter(Boolean)[0] ?? '';
}

function afterRouteRendered(callback: () => void): void {
    requestAnimationFrame(() => {
        requestAnimationFrame(callback);
    });
}

function scrollSidebar(path: string, packageChanged: boolean): void {
    const sidebar = document.querySelector<HTMLElement>('.VPSidebar');
    if (!sidebar) return;

    if (packageChanged) {
        scrollToTop(sidebar);
        scrollActiveSidebarLink(path, true);
        return;
    }

    scrollActiveSidebarLink(path, false);
}

function scrollActiveSidebarLink(path: string, force: boolean): void {
    for (const delay of force ? [100, 250, 500] : [0]) {
        window.setTimeout(() => {
            const sidebar = document.querySelector<HTMLElement>('.VPSidebar');
            if (!sidebar) return;

            const activeLink = findSidebarLink(sidebar, path);
            if (!activeLink) return;

            if (force || !isElementVisible(activeLink, sidebar)) {
                scrollElementIntoSidebar(sidebar, activeLink);
            }
        }, delay);
    }
}

function findSidebarLink(sidebar: HTMLElement, path: string): HTMLElement | null {
    const normalizedPath = normalizePath(path);
    const links = Array.from(sidebar.querySelectorAll('a[href]')) as HTMLAnchorElement[];

    for (const link of links) {
        if (normalizePath(link.getAttribute('href') ?? '') === normalizedPath) {
            return link.closest('.VPSidebarItem') as HTMLElement | null ?? link;
        }
    }

    return null;
}

function normalizePath(path: string): string {
    return `/${path.split('#')[0].split('?')[0].split('/').filter(Boolean).join('/')}`;
}

function scrollElementIntoSidebar(sidebar: HTMLElement, element: HTMLElement): void {
    const sidebarRect = sidebar.getBoundingClientRect();
    const elementRect = element.getBoundingClientRect();
    const top = sidebar.scrollTop + elementRect.top - sidebarRect.top - (sidebar.clientHeight / 2) + (element.clientHeight / 2);

    sidebar.scrollTo({
        top: Math.max(0, top),
        behavior: 'smooth'
    });
}

function isElementVisible(element: HTMLElement, container: HTMLElement): boolean {
    const elementRect = element.getBoundingClientRect();
    const containerRect = container.getBoundingClientRect();

    return elementRect.top >= containerRect.top && elementRect.bottom <= containerRect.bottom;
}
