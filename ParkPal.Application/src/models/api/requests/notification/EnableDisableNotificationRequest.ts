export default class EnableDisableNotificationRequest {
    attractionTimerId?: number;

    constructor(data: Pick<EnableDisableNotificationRequest, "attractionTimerId"> | null = null) {
        if(data != null) {
            this.attractionTimerId = data.attractionTimerId;
        }
    }
}