export default class EditNotificationRequest {
    attractionTimerId?: number;
    criteriaType?: number;
    waitTime?: number;

    constructor(data: Pick<EditNotificationRequest, "attractionTimerId" | "criteriaType" | "waitTime"> | null = null) {
        if(data != null) {
            this.attractionTimerId = data.attractionTimerId;
            this.criteriaType = data.criteriaType;
            this.waitTime = data.waitTime;
        }
    }
}