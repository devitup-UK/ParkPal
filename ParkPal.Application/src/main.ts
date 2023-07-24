import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import * as Sentry from "@sentry/vue";
import { BrowserTracing } from "@sentry/tracing";

import { IonicVue } from '@ionic/vue';

import { library } from '@fortawesome/fontawesome-svg-core'
import {
    faMapMarkerAlt,
    faClock,
    faCog,
    faMapMarker,
    faFilter,
    faHeart,
    faArrowLeft,
    faChevronRight,
    faChevronDown,
    faTimesCircle,
    faLocationDot,
    faMoon,
    faPlus,
    faAt,
    faWallet,
    faPaintRoller,
    faTrash,
    faBell,
    faBellSlash,
    faSpinner,
    faLink,
    faStar,
    faComments,
    faBook,
    faUserSecret,
    faArrowDownWideShort,
    faCartShopping
} from '@fortawesome/free-solid-svg-icons';

import {faFacebook, faInstagram, faTwitter} from "@fortawesome/free-brands-svg-icons";

library.add(faMapMarkerAlt,
    faClock,
    faCog,
    faMapMarker,
    faFilter,
    faHeart,
    faArrowLeft,
    faChevronRight,
    faChevronDown,
    faTimesCircle,
    faLocationDot,
    faMoon,
    faPlus,
    faAt,
    faWallet,
    faTimesCircle,
    faPaintRoller,
    faTrash,
    faBell,
    faBellSlash,
    faSpinner,
    faLink,
    faStar,
    faComments,
    faBook,
    faUserSecret,
    faArrowDownWideShort,
    faFacebook,
    faTwitter,
    faInstagram,
    faCartShopping)



/* Core CSS required for Ionic components to work properly */
import '@ionic/vue/css/core.css';

/* Basic CSS for apps built with Ionic */
import '@ionic/vue/css/normalize.css';
import '@ionic/vue/css/structure.css';
import '@ionic/vue/css/typography.css';

/* Optional CSS utils that can be commented out */
import '@ionic/vue/css/padding.css';
import '@ionic/vue/css/float-elements.css';
import '@ionic/vue/css/text-alignment.css';
import '@ionic/vue/css/text-transformation.css';
import '@ionic/vue/css/flex-utils.css';
import '@ionic/vue/css/display.css';

/* Index variables */
import './theme/variables.css';

import Vue3TouchEvents from "vue3-touch-events";


const app = createApp(App).use(IonicVue).use(router).use(store).use(Vue3TouchEvents, {
    swipeTolerance: 3
});

Sentry.init({
    app,
    dsn: "https://452cf5e9dd8445ac912605f95c64fbc2@o261761.ingest.sentry.io/4504207472721920",
    integrations: [
        new BrowserTracing({
            routingInstrumentation: Sentry.vueRouterInstrumentation(router),
            tracePropagationTargets: ["localhost", "capacitor", /^\//],
        }),
    ],
    // Set tracesSampleRate to 1.0 to capture 100%
    // of transactions for performance monitoring.
    // We recommend adjusting this value in production
    tracesSampleRate: 1.0,
});

router.isReady().then(() => {
    app.mount('#app');
});



