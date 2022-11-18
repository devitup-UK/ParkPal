export default class SaveSubscriptionRequest {
    playerId?: string;

    constructor(data: Pick<SaveSubscriptionRequest, "playerId"> | null = null) {
        if(data != null) {
            this.playerId = data.playerId;
        }

    }
}