export default class EnableDisableNotificationRequest {
    notificationId?: number;

    constructor(data: Pick<EnableDisableNotificationRequest, "notificationId"> | null = null) {
        if(data != null) {
            this.notificationId = data.notificationId;
        }
    }
}