import AttractionTimer from "@/models/api/AttractionTimer";

export default class Subscription {
    subscriptionId?: number;
    tokenId?: number;
    token?: Token;
    playerId?: string;
    attractionTimers: Array<AttractionTimer> = [];

    constructor(data: Pick<Subscription, "subscriptionId" | "tokenId" | "token" | "playerId" | "attractionTimers"> | null = null) {
        if(data != null) {
            this.subscriptionId = data.subscriptionId;
            this.tokenId = data.tokenId;
            this.token = data.token;
            this.playerId = data.playerId;

            if(data.attractionTimers) {
                data.attractionTimers.forEach((attractionTimer: AttractionTimer) => {
                    this.attractionTimers.push(attractionTimer);
                });
            }
        }
    }
}