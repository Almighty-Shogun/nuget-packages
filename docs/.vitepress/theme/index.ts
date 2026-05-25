import type { Theme } from 'vitepress'
import DefaultTheme from 'vitepress/theme'
import { registerRouteScroll } from './routeScroll'
import { FrontmatterDocs, Parameters, Returns, Fields } from './components'

import './custom.css'
import 'virtual:group-icons.css'

export default {
    extends: DefaultTheme,
    enhanceApp({ app, router }) {
        app.component('Fields', Fields);
        app.component('Returns', Returns);
        app.component('Parameters', Parameters);
        app.component('FrontmatterDocs', FrontmatterDocs);

        registerRouteScroll(router);
    }
} satisfies Theme;
