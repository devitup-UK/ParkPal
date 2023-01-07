export default class EditNotificationRequest {
    notificationId?: number;
    criteriaType?: number;
    waitTime?: number;

    constructor(data: Pick<EditNotificationRequest, "notificationId" | "criteriaType" | "waitTime"> | null = null) {
        if(data != null) {
            this.notificationId = data.notificationId;
            this.criteriaType = data.criteriaType;
            this.waitTime = data.waitTime;
        }
    }
}