
import Vuex, { StoreOptions } from 'vuex'

import { RootState } from './types';
import {themeparkService} from "@/services/themepark.service";
import {AxiosResponse} from "axios";
import destinationTransformer from '../transformers';
import Settings from "@/models/store/Settings";
import {storageService} from "@/services/storage.service";
import WaitTimeFilter from "@/models/store/WaitTimeFilter";
import NotificationHoldingArea from "@/models/store/NotificationHoldingArea";
import {notificationService} from "@/services/notification.service";
import CreateNotificationRequest from "@/models/api/requests/notification/CreateNotificationRequest";
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
import {PurchasesPackage} from "cordova-plugin-purchases";

const store: StoreOptions<RootState> = {
  state: {
    destinations: [],
    activeDestination: null,
    activePark: null,
    settings: new Settings(),
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
      storageService.storeSettingsInLocalStorage(state.settings);
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
    setProducts(state, products: Array<PurchasesPackage>) {
      state.products = products;
    },
    setDarkMode(state) {
      state.settings.theme.darkMode = true;
      state.settings.theme.setDarkTheme();
      storageService.storeSettingsInLocalStorage(state.settings);
    },
    setLightMode(state) {
      state.settings.theme.darkMode = false;
      state.settings.theme.setLightTheme();
      storageService.storeSettingsInLocalStorage(state.settings);
    },
    setTheme(state, theme: Theme) {
      state.settings.theme = theme;
      storageService.storeSettingsInLocalStorage(state.settings);
    }
  },
  actions: {
    getDestinations({ commit }, forceRefresh = false) {
      commit('setServerError', false);
      return new Promise((resolve) => {
        if(forceRefresh) {
          themeparkService.getDestinations().then(
              (response: AxiosResponse<Array<Destination>>) => {
                destinationTransformer.transformApiDestinationArrayToInternalDestinationArray(response.data).then((destinations) => {
                  commit('setServerError', false);
                  commit("setDestinations", destinations);
                  resolve(destinations);
                });
              })
              .catch(() => {
                commit('setServerError', true);
              });
        }
      })
    },
    configureStorage({ commit }) {
      // This function sets up our store using Local Storage.
      // Settings
      const settings: Settings = storageService.getSettingsFromLocalStorage();
      commit('setSettings', settings);

      // ActiveDestination
      const activeDestination: Destination = storageService.getActiveDestinationFromLocalStorage();
      commit('setActiveDestination', activeDestination);

      // ActivePark
      const activePark: Park = storageService.getActiveParkFromLocalStorage();
      commit('setActivePark', activePark);
    },
    overwriteSettings({commit}, settings: Settings) {
      commit('setSettings', settings);
      storageService.storeSettingsInLocalStorage(settings);
    },
    reSaveSettings({state}) {
      storageService.storeSettingsInLocalStorage(state.settings);
    },
    addFavourite({commit, state}, id: string) {
      commit('addFavourite', id);
      storageService.storeSettingsInLocalStorage(state.settings);
    },
    removeFavourite({commit, state}, id: string) {
      commit('removeFavourite', id);
      storageService.storeSettingsInLocalStorage(state.settings);
    },
    addNotification({dispatch, state}, notificationToCreate: CreateNotificationRequest) {
      notificationService.createNotification(notificationToCreate).then(() => {
        dispatch('getAllNotifications', {
          filters: state.filters.notificationsFilter,
          favouriteAttractionIds: state.settings.favourites
        })
      });
    },
    editNotification({dispatch, state}, request: EditNotificationRequest) {
      notificationService.editNotification(request).then(() => {
        dispatch('getAllNotifications', {
          filters: state.filters.notificationsFilter,
          favouriteAttractionIds: state.settings.favourites
        })
      });
    },
    deleteNotification({dispatch, state}, attractionTimerId: number) {
      notificationService.deleteNotification(attractionTimerId).then(deleted => {
        if(deleted) {
          dispatch('getAllNotifications', {
            filters: state.filters.notificationsFilter,
            favouriteAttractionIds: state.settings.favourites
          })
        }
      });
    },
    getAllNotifications({ commit }, request: GetNotificationsRequest) {
      commit('setServerError', false);
      notificationService.getAllNotifications(request).then(timers => {
        const transformedTimers = transformers.transformApiTimerWithAttractionArrayToInternalTimerWithAttractionArray(timers);
        commit('setNotifications', transformedTimers);
      }).catch(() => {
        commit('setServerError', true);
      })
    },
    setToken({commit}, token: string) {
      commit('setToken', token);
    },
    setActiveDestination({commit}, destination: Destination) {
      commit('setActiveDestination', destination);
      storageService.storeActiveDestinationInLocalStorage(destination);
    },
    setActivePark({commit}, park: Park) {
      commit('setActivePark', park);
      storageService.storeActiveParkInLocalStorage(park);
    },
    setNotificationsFilter({commit}, filters: NotificationsFilter) {
      commit('setNotificationsFilter', filters);
    },
    toggleDestination({commit, state}, destinationId: string) {
      commit('toggleDestination', destinationId);
      storageService.storeSettingsInLocalStorage(state.settings);
    },
    setNotificationsEnabled({commit}, notificationsEnabled: boolean) {
      commit('setNotificationsEnabled', notificationsEnabled);
    },
    setParkPalPlus({commit}, value: boolean) {
      commit('setParkPalPlus', value);
      storageService.storeSettingsInLocalStorage(this.state.settings);
    },
    setServerError({commit}, value: boolean) {
      commit('setServerError', value);
    },
    setProducts({commit}, products: Array<PurchasesPackage>) {
      commit('setProducts', products);
    },
    setDarkMode({commit}) {
      commit('setDarkMode');
    },
    setLightMode({commit}) {
      commit('setLightMode');
    },
    setTheme({commit}, theme: Theme) {
      commit('setTheme', theme);
    }

  },
}

export default new Vuex.Store<RootState>(store);
