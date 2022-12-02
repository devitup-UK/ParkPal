import NotificationsFilter from "@/models/store/NotificationsFilter";

export default class GetNotificationsRequest {
    filters?: NotificationsFilter = new NotificationsFilter();
    favouriteIds?: Array<string> = [];

    constructor(data: Pick<GetNotificationsRequest, "filters" | "favouriteIds"> | null = null) {
        if(data != null) {
            this.filters = data.filters;
            this.favouriteIds = data.favouriteIds;
        }
    }
}