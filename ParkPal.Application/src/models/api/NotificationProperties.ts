export default class NotificationProperties {
    itemId = 0;
    typeId = 1;
    minuteInterval = 5;
    parkId?: string;
    attractionId?: string | null = null;
    criteriaType?: number = 1;
    waitTime = 35;
    enabled = true;

    constructor(data: Pick<NotificationProperties, "itemId" | "typeId" | "minuteInterval" | "parkId" | "attractionId" | "criteriaType" | "waitTime" | "enabled"> | null = null) {
        if(data != null) {
            this.itemId = data.itemId;
            this.typeId = data.typeId;
            this.minuteInterval = data.minuteInterval;
            this.parkId = data.parkId;
            this.attractionId = data.attractionId;
            this.criteriaType = data.criteriaType;
            this.waitTime = data.waitTime;
            this.enabled = data.enabled;
        }
    }
}