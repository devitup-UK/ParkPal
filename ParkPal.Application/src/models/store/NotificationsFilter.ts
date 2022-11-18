import {NotificationsFilterCriteria} from "@/models/enums/NotificationsFilterCriteria";
import {NotificationsFilterType} from "@/models/enums/NotificationsFilterType";
import {NotificationsFilterSort} from "@/models/enums/NotificationsFilterSort";

export default class NotificationsFilter {
    criteria: NotificationsFilterCriteria = NotificationsFilterCriteria.Any;
    type: NotificationsFilterType = NotificationsFilterType.AllAttractions;
    sort: NotificationsFilterSort = NotificationsFilterSort.HighestWaitTime;
    parkId: string | null = null;

    constructor(data: Pick<NotificationsFilter, "criteria" | "type" | "sort" | "parkId"> | null = null) {
        if(data != null) {
            this.criteria = data.criteria;
            this.type = data.type;
            this.sort = data.sort;
            this.parkId = data.parkId;
        }

    }
}