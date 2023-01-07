import { createRouter, createWebHistory } from '@ionic/vue-router';
import { RouteRecordRaw } from 'vue-router';

import SettingsIndexView from '../views/tabs/Settings/IndexView.vue';
import SettingsManageDestinationsView from '../views/tabs/Settings/ManageDestinationsView.vue';
import SettingsAboutAndFAQsView from '../views/tabs/Settings/AboutAndFAQsView.vue';
import SettingsCustomThemingView from '../views/tabs/Settings/CustomTheming.vue';

import NotificationsView from '../views/tabs/Notifications/IndexView.vue';
import NotificationsCreateView from '../views/tabs/Notifications/NotificationsCreateView.vue';
import NotificationFiltersView from "@/views/tabs/Notifications/NotificationFiltersView.vue";
import NotificationsEditView from '../views/tabs/Notifications/NotificationsEditView.vue';

import IndexView from '../views/tabs/Destinations/IndexView.vue';
import ParksView from '../views/tabs/Destinations/ParksView.vue';
import WaitTimesView from "@/views/tabs/Destinations/WaitTimesView.vue";
import WaitTimesFilterView from "@/views/tabs/Destinations/WaitTimesFilterView.vue";

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    redirect: '/tabs/destinations'
  },
  {
    name: 'destinations',
    path: '/tabs/destinations',
    component: IndexView
  },
  {
    name: 'notifications',
    path: '/tabs/notifications',
    component: NotificationsView
  },
  {
    name: 'notificationFilters',
    path: '/tabs/notifications/filters',
    component: NotificationFiltersView
  },
  {
    name: 'parks',
    path: '/tabs/destinations/parks',
    component: ParksView
  },
  {
    name: 'waitTimes',
    path: '/tabs/destinations/parks/waitTimes',
    component: WaitTimesView
  },
  {
    name: 'waitTimeFilters',
    path: '/tabs/destinations/parks/waitTimes/filters',
    component: WaitTimesFilterView
  },
  {
    name: 'notificationsCreate',
    path: '/tabs/notifications/create',
    component: NotificationsCreateView
  },
  {
    name: 'notificationsEdit',
    path: '/tabs/notifications/edit/:notificationId',
    component: NotificationsEditView
  },
  {
    name: 'settings',
    path: '/tabs/settings',
    component: SettingsIndexView
  },
  {
    name: 'settingsManageDestinations',
    path: '/tabs/settings/destinations',
    component: SettingsManageDestinationsView
  },
  {
    name: 'settingsAboutAndFAQs',
    path: '/tabs/settings',
    component: SettingsAboutAndFAQsView
  },
  {
    name: 'settingsCustomTheming',
    path: '/tabs/settings/customTheming',
    component: SettingsCustomThemingView
  }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
