import Settings from "@/models/store/Settings";
import WaitTimeFilter from "@/models/store/WaitTimeFilter";
import NotificationHoldingArea from "@/models/store/NotificationHoldingArea";
import TimerWithAttraction from "@/models/api/TimerWithAttraction";
import Destination from "@/models/api/Destination";
import NotificationsFilter from "@/models/store/NotificationsFilter";
import Park from "@/models/api/Park";
import {PurchasesPackage} from "cordova-plugin-purchases";

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
    products: Array<PurchasesPackage>;
}