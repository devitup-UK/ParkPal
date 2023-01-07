export default class CreateNotificationRequest {
    attractionId?: string | null;
    type?: number;
    parkId?: string;
    minuteInterval?: number = 5;
    criteriaType?: number;
    waitTime?: number;

    constructor(data: Pick<CreateNotificationRequest, "attractionId" | "type" | "parkId" | "minuteInterval" | "criteriaType" | "waitTime"> | null = null) {
        if(data != null) {
            this.attractionId = data.attractionId;
            this.type = data.type;
            this.parkId = data.parkId;
            this.minuteInterval = data.minuteInterval;
            this.criteriaType = data.criteriaType;
            this.waitTime = data.waitTime;
        }
    }
}