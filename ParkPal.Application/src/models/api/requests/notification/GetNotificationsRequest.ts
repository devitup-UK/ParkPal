import NotificationsFilter from "@/models/store/NotificationsFilter";

export default class GetNotificationsRequest {
    filters?: NotificationsFilter = new NotificationsFilter();
    favouriteAttractionIds?: Array<string> = [];

    constructor(data: Pick<GetNotificationsRequest, "filters" | "favouriteAttractionIds"> | null = null) {
        if(data != null) {
            this.filters = data.filters;
            this.favouriteAttractionIds = data.favouriteAttractionIds;
        }
    }
}