import Settings from "@/models/store/Settings";
import WaitTimeFilter from "@/models/store/WaitTimeFilter";
import NotificationHoldingArea from "@/models/store/NotificationHoldingArea";
import Notification from "@/models/api/Notification";
import Destination from "@/models/api/Destination";
import NotificationsFilter from "@/models/store/NotificationsFilter";
import Park from "@/models/api/Park";

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
    notifications: Array<Notification>;
    serverError: boolean;
    destinationSlideIndex: number;
    destinationSearchTerm: string;
    modalOpen: boolean;
    adHeight: number;
    keyboard: boolean;
    advertisementsInitialised: boolean;
}