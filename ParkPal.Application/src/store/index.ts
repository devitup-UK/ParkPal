
import Vuex, { StoreOptions } from 'vuex'

import { RootState } from './types';
import {themeparkService} from "@/services/themepark.service";
import {AxiosResponse} from "axios";
import destinationTransformer from '../transformers';
import Settings from "@/models/store/Settings";
import {settingsService} from "@/services/settings.service";
import WaitTimeFilter from "@/models/store/WaitTimeFilter";
import NotificationHoldingArea from "@/models/store/NotificationHoldingArea";
import {notificationService} from "@/services/notification.service";
import CreateNotificationRequest from "@/models/api/requests/notification/CreateNotificationRequest";
import EnableDisableNotificationRequest from "@/models/api/requests/notification/EnableDisableNotificationRequest";
import transformers from '../transformers';
import TimerWithAttraction from "@/models/api/TimerWithAttraction";
import Destination from "@/models/api/Destination";
import GetNotificationsRequest from "@/models/api/requests/notification/GetNotificationsRequest";
import NotificationsFilter from "@/models/store/NotificationsFilter";
import EditNotificationRequest from "@/models/api/requests/notification/EditNotificationRequest";
import Park from "@/models/api/Park";
import {IAPProduct} from "@ionic-native/in-app-purchase-2";
import {Package} from "@capgo/capacitor-purchases";
import Theme from "@/models/store/theme/Theme";

const store: StoreOptions<RootState> = {
  state: {
    destinations: [],
    activeDestination: null,
    activePark: null,
    settings: new Settings(),
    // Store the filters here, we can apply them when they come back from the API.
    filters: {
      waitTimeFilter: new WaitTimeFilter(),
      notificationsFilter: new NotificationsFilter()
    },
    notificationHoldingArea: new NotificationHoldingArea(),
    loading: false,
    notifications: [],
    isApp: !document.URL.startsWith('http'),
    notificationsEnabled: false,
    serverError: false,
    products: []
  },
  getters: {
    favourites(state) {
      return state.settings.favourites;
    },
    notifications(state) {
      return state.notifications;
    },
    notificationIds(state) {
      const ids: Array<string> = [];

      state.notifications.forEach(notification => {
        if(notification.timer?.attractionId) {
          ids.push(notification.timer.attractionId);
        }
      })

      return ids;
    }
  },
  mutations: {
    isLoading(state, isLoading: boolean) {
      state.loading = isLoading;
    },
    setActiveDestination(state, destination: Destination) {
      state.activeDestination = destination;
    },
    setDestinations(state, destinations: Array<Destination>) {
      state.destinations = destinations;
    },
    setSettings(state, settings: Settings) {
      state.settings = settings;
    },
    addFavourite(state, id: string) {
      state.settings.favourites.push(id);
    },
    removeFavourite(state, id: string) {
      const index = state.settings.favourites.indexOf(id);
      state.settings.favourites.splice(index, 1);
    },
    setWaitTimeFilter(state, filter: WaitTimeFilter) {
      state.filters.waitTimeFilter = filter;
    },
    resetWaitTimeFilter(state) {
      state.filters.waitTimeFilter = new WaitTimeFilter();
    },
    clearNotificationHoldingArea(state) {
      state.notificationHoldingArea = null;
    },
    setNotificationHoldingArea(state, notificationHoldingArea: NotificationHoldingArea) {
      state.notificationHoldingArea = notificationHoldingArea;
    },
    addNotification(state, notification: TimerWithAttraction) {
      state.notifications.push(notification);
    },
    setNotifications(state, timers: Array<TimerWithAttraction>) {
      state.notifications = timers;
    },
    setToken(state, token: string) {
      state.settings.apiToken = token;
    },
    setActivePark(state, park: Park) {
      state.activePark = park;
    },
    setNotificationsFilter(state, filters: NotificationsFilter) {
      state.filters.notificationsFilter = filters;
    },
    toggleDestination(state, destinationId: string) {
      if(state.settings.hiddenDestinations.includes(destinationId)) {
        const destinationIndex = state.settings.hiddenDestinations.indexOf(destinationId);
        state.settings.hiddenDestinations.splice(destinationIndex, 1);
      }else{
        state.settings.hiddenDestinations.push(destinationId);
      }
    },
    setNotificationsEnabled(state, notificationsEnabled: boolean) {
      state.notificationsEnabled = notificationsEnabled;
    },
    setParkPalPlus(state, value: boolean) {
      state.settings.parkPalPlus = value;
    },
    setServerError(state, value: boolean) {
      state.serverError = value;
    },
    setProducts(state, products: Array<Package>) {
      state.products = products;
    },
    setDarkMode(state) {
      state.settings.theme.darkMode = true;
      state.settings.theme.setDarkTheme();
      settingsService.storeSettingsInLocalStorage(state.settings);
    },
    setLightMode(state) {
      state.settings.theme.darkMode = false;
      state.settings.theme.setLightTheme();
      settingsService.storeSettingsInLocalStorage(state.settings);
    },
    setTheme(state, theme: Theme) {
      state.settings.theme = theme;
      settingsService.storeSettingsInLocalStorage(state.settings);
    }
  },
  actions: {
    getDestinations({ commit, dispatch }, forceRefresh = false) {
      commit('setServerError', false);
      return new Promise((resolve, reject) => {
        if(forceRefresh) {
          themeparkService.getDestinations().then(
              (response: AxiosResponse<Array<Destination>>) => {
                destinationTransformer.transformApiDestinationArrayToInternalDestinationArray(response.data).then((destinations) => {
                  commit('setServerError', false);
                  commit("setDestinations", destinations);
                  resolve(destinations);

                });

                })
              .catch((error) => {
                commit('setServerError', true);
                // reject(error.response)
              });
        }
      })
    },
    configureSettings({ commit, dispatch }) {
      const settings: Settings = settingsService.getSettingsFromLocalStorage();
      commit('setSettings', settings);
      settingsService.storeSettingsInLocalStorage(settings);
    },
    overwriteSettings({commit, dispatch}, settings: Settings) {
      commit('setSettings', settings);
      settingsService.storeSettingsInLocalStorage(settings);
    },
    reSaveSettings({commit, dispatch}) {
      settingsService.storeSettingsInLocalStorage(this.state.settings);
    },
    addFavourite({commit, dispatch}, id: string) {
      commit('addFavourite', id);
      settingsService.storeSettingsInLocalStorage(this.state.settings);
    },
    removeFavourite({commit, dispatch}, id: string) {
      commit('removeFavourite', id);
      settingsService.storeSettingsInLocalStorage(this.state.settings);
    },
    addNotification({commit, dispatch, state}, notificationToCreate: CreateNotificationRequest) {
      // Call our API to create a notification.
      notificationService.createNotification(notificationToCreate).then(attractionTimer => {
        dispatch('getAllNotifications', {
          filters: state.filters.notificationsFilter,
          favouriteAttractionIds: state.settings.favourites
        })
      });
    },
    editNotification({commit, dispatch, state}, request: EditNotificationRequest) {
      // Call our API to create a notification.
      notificationService.editNotification(request).then(() => {
        dispatch('getAllNotifications', {
          filters: state.filters.notificationsFilter,
          favouriteAttractionIds: state.settings.favourites
        })
      });
    },
    deleteNotification({commit, dispatch, state}, attractionTimerId: number) {
      // Call our API to delete the notification.
      notificationService.deleteNotification(attractionTimerId).then(deleted => {
        if(deleted) {
          dispatch('getAllNotifications', {
            filters: state.filters.notificationsFilter,
            favouriteAttractionIds: state.settings.favourites
          })
        }
      });
    },
    getAllNotifications({ commit, dispatch }, request: GetNotificationsRequest) {
      // Call the API to get all the notifications.
      commit('setServerError', false);
      notificationService.getAllNotifications(request).then(timers => {
        const transformedTimers = transformers.transformApiTimerWithAttractionArrayToInternalTimerWithAttractionArray(timers);
        commit('setNotifications', transformedTimers);
      }).catch((error) => {
        commit('setServerError', true);
      })
    },
    setToken({commit, dispatch}, token: string) {
      commit('setToken', token);
    },
    setActiveDestination({commit, dispatch}, destination: Destination) {
      commit('setActiveDestination', destination);
    },
    setActivePark({commit, dispatch}, park: Park) {
      commit('setActivePark', park);
    },
    setNotificationsFilter({commit, dispatch}, filters: NotificationsFilter) {
      commit('setNotificationsFilter', filters);
    },
    toggleDestination({commit, dispatch, state}, destinationId: string) {
      commit('toggleDestination', destinationId);
      settingsService.storeSettingsInLocalStorage(state.settings);
    },
    setNotificationsEnabled({commit, dispatch}, notificationsEnabled: boolean) {
      commit('setNotificationsEnabled', notificationsEnabled);
    },
    setParkPalPlus({commit, dispatch}, value: boolean) {
      commit('setParkPalPlus', value);
      settingsService.storeSettingsInLocalStorage(this.state.settings);
    },
    setServerError({commit, dispatch}, value: boolean) {
      commit('setServerError', value);
    },
    setProducts({commit, dispatch}, products: {[p: string]: IAPProduct}) {
      commit('setProducts', products);

      let owned = false;

      // Now we have all the products, we can check if any are owned/subscribed.
      for (const [key, value] of Object.entries(products)) {
        if(!owned) {
          owned = value.owned;
        }
      }

      commit('setParkPalPlus', owned);

      settingsService.storeSettingsInLocalStorage(this.state.settings);
    },
    setDarkMode({commit, dispatch}) {
      commit('setDarkMode');
    },
    setLightMode({commit, dispatch}) {
      commit('setLightMode');
    },
    setTheme({commit, dispatch}, theme: Theme) {
      commit('setTheme', theme);
    }

  },
}

export default new Vuex.Store<RootState>(store);
