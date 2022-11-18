import Settings from "@/models/store/Settings";
import WaitTimeFilter from "@/models/store/WaitTimeFilter";
import AttractionTimer from "@/models/api/AttractionTimer";
import NotificationHoldingArea from "@/models/store/NotificationHoldingArea";
import TimerWithAttraction from "@/models/api/TimerWithAttraction";
import Destination from "@/models/api/Destination";
import NotificationsFilter from "@/models/store/NotificationsFilter";
import Park from "@/models/api/Park";
import {IAPProduct} from "@ionic-native/in-app-purchase-2";
import {Package} from "@capgo/capacitor-purchases";

export interface RootState {
    destinations: Array<Destination>;
    activeDestination: Destination | null;
    activePark: Park | null;
    settings: Settings;
    filters: {
        waitTimeFilter: WaitTimeFilter,
        notificationsFilter: NotificationsFilter
    };
    notificationHoldingArea: NotificationHoldingArea | null,
    loading: boolean;
    isApp: boolean;
    notificationsEnabled: boolean;
    notifications: Array<TimerWithAttraction>;
    serverError: boolean;
    products: Array<Package>;
}