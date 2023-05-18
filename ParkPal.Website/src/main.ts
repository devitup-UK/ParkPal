import Vue from 'vue'
import App from './App.vue'
import store from './store'
import { BootstrapVue } from 'bootstrap-vue'
import Ads from 'vue-google-adsense'

import { library } from '@fortawesome/fontawesome-svg-core'
import { faClock,  faSearch, faBell, faHeart, faPaintRoller, faAppleWhole, faAt, faCode, faPhotoFilm, faStar, faPhone, faLaptopCode, faMobileAlt, faPaintBrush, faMapMarkerAlt, faEnvelope, faShareAlt, faTimes, faExclamationCircle, faMobile } from '@fortawesome/free-solid-svg-icons'
import { faApple, faFacebook, faTwitter, faInstagram, faPinterest, faAndroid } from '@fortawesome/free-brands-svg-icons'
import { FontAwesomeIcon, FontAwesomeLayers, FontAwesomeLayersText } from '@fortawesome/vue-fontawesome'
import Adsense from "vue-google-adsense/dist/ads/Adsense";

Vue.config.productionTip = false

Vue.component('font-awesome-icon', FontAwesomeIcon)
Vue.component('font-awesome-layers', FontAwesomeLayers)
Vue.component('font-awesome-layers-text', FontAwesomeLayersText)

library.add(faAndroid, faSearch, faBell, faHeart, faClock, faPaintRoller, faApple, faAt, faPhotoFilm, faStar, faMobile, faCode, faPhone, faFacebook, faTwitter, faInstagram, faPinterest, faLaptopCode, faMobileAlt, faPaintBrush, faMapMarkerAlt, faEnvelope, faShareAlt, faTimes, faExclamationCircle)

Vue.use(BootstrapVue);
// eslint-disable-next-line
Vue.use(require('vue-script2'));
Vue.use(Ads.Adsense);

new Vue({
  store,
  render: h => h(App)
}).$mount('#app')
