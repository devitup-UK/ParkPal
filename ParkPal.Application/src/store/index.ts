
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
import Notification from "@/models/api/Notification";
import Destination from "@/models/api/Destination";
import GetNotificationsRequest from "@/models/api/requests/notification/GetNotificationsRequest";
import NotificationsFilter from "@/models/store/NotificationsFilter";
import EditNotificationRequest from "@/models/api/requests/notification/EditNotificationRequest";
import Park from "@/models/api/Park";
import Theme from "@/models/store/theme/Theme";
import {PurchasesPackage} from "cordova-plugin-purchases";
import EnableDisableNotificationRequest from "@/models/api/requests/notification/EnableDisableNotificationRequest";

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
    products: [],
    destinationSlideIndex: 0,
    destinationSearchTerm: '',
    modalOpen: false,
    adHeight: 60
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
        if(notification.properties?.attractionId) {
          ids.push(notification.properties.attractionId);
        }

        if(notification.properties?.parkId && notification.properties.attractionId == '') {
          ids.push(notification.properties.parkId);
        }
      })

      return ids;
    },
    notificationAttractionIds(state) {
      const ids: Array<string> = [];

      state.notifications.forEach(notification => {
        if(notification.properties?.attractionId) {
          ids.push(notification.properties.attractionId);
        }
      })

      return ids;
    },
    notificationParkIds(state) {
      const ids: Array<string> = [];

      state.notifications.forEach(notification => {
        if(notification.properties?.parkId && notification.properties.attractionId == null) {
          ids.push(notification.properties.parkId);
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
    addNotification(state, notification: Notification) {
      state.notifications.push(notification);
    },
    setNotifications(state, timers: Array<Notification>) {
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
    enableAllDestinations(state) {
      state.settings.hiddenDestinations = [];
    },
    disableAllDestinations(state, destinations: Array<Destination>) {
      destinations.forEach((destination: Destination) => {
        state.settings.hiddenDestinations.push(destination.destinationId);
      })
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
      state.settings.theme.setDarkTheme();
      storageService.storeSettingsInLocalStorage(state.settings);
    },
    setLightMode(state) {
      state.settings.theme.setLightTheme();
      storageService.storeSettingsInLocalStorage(state.settings);
    },
    setTheme(state, theme: Theme) {
      state.settings.theme = theme;
      storageService.storeSettingsInLocalStorage(state.settings);
    },
    setDestinationSlideIndex(state, activeIndex: number) {
      state.destinationSlideIndex = activeIndex;
    },
    setDestinationSearchTerm(state, searchTerm: string) {
      state.destinationSearchTerm = searchTerm;
    },
    setModalOpen(state, modalOpen: boolean) {
      state.modalOpen = modalOpen;
    },
    setNotificationsRequested(state, value: boolean) {
      state.settings.requestedNotifications = value;
      storageService.storeSettingsInLocalStorage(state.settings);
    },
    setAdHeight(state, value: number) {
      console.log('settingAdHeight through mutation', value);
      state.adHeight = value;
      console.log('adHeight is now in state', value);
    },
    setVoucher(state, value: string | undefined) {
      state.settings.voucher = value;
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
          favouriteIds: state.settings.favourites
        })
      });
    },
    editNotification({dispatch, state}, request: EditNotificationRequest) {
      notificationService.editNotification(request).then(() => {
        dispatch('getAllNotifications', {
          filters: state.filters.notificationsFilter,
          favouriteIds: state.settings.favourites
        })
      });
    },
    deleteNotification({dispatch, state}, notificationId: number) {
      notificationService.deleteNotification(notificationId).then(deleted => {
        if(deleted) {
          dispatch('getAllNotifications', {
            filters: state.filters.notificationsFilter,
            favouriteIds: state.settings.favourites
          })
        }
      });
    },
    getAllNotifications({ commit }, request: GetNotificationsRequest) {
      commit('setServerError', false);
      notificationService.getAllNotifications(request).then(timers => {
        const transformedTimers = transformers.transformApiNotificationWithEntityArrayToInternalNotificationWithEntityArray(timers);
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
    },
    setNotificationEnabled({state, dispatch}, notificationId: number) {
      return new Promise((resolve) => {
        notificationService.enableNotification(new EnableDisableNotificationRequest({
          notificationId
        })).then(() => {
          dispatch('getAllNotifications', {
            filters: state.filters.notificationsFilter,
            favouriteIds: state.settings.favourites
          });
          resolve(true);
        });
      });
    },
    setNotificationDisabled({state, dispatch}, notificationId: number) {
      return new Promise((resolve) => {
        notificationService.disableNotification(new EnableDisableNotificationRequest({
          notificationId
        })).then(() => {
          dispatch('getAllNotifications', {
            filters: state.filters.notificationsFilter,
            favouriteIds: state.settings.favourites
          })
          resolve(true);
        });
      });
    },
    setDestinationSlideIndex({commit}, slideIndex: number) {
      commit('setDestinationSlideIndex', slideIndex);
    },
    setDestinationSearchTerm({commit}, searchTerm: string) {
      commit('setDestinationSearchTerm', searchTerm);
    },
    setModalOpen({commit}, modalOpen: boolean) {
      commit('setModalOpen', modalOpen);
    },
    setNotificationsRequested({commit}, value: boolean) {
      commit('setNotificationsRequested', value);
    },
    setAdHeight({commit}, value: number) {
      console.log('settingAdHeight through dispatch', value);
      commit('setAdHeight', value);
    },
    enableAllDestinations({commit}) {
      commit('enableAllDestinations');
    },
    disableAllDestinations({commit}, destinations: Array<Destination>) {
      commit('disableAllDestinations', destinations);
    },
    setVoucher({commit}, value: string) {
      commit('setVoucher', value);
    },
    removeVoucher({commit}) {
      commit('setVoucher', undefined);
    }
  },
}

export default new Vuex.Store<RootState>(store);
