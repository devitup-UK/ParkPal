import Attraction from "@/models/api/Attraction";
import Park from "@/models/api/Park";


export default class NotificationHoldingArea {
    attraction: Attraction = new Attraction();
    park: Park = new Park();

    constructor(data: Pick<NotificationHoldingArea, "attraction" | "park"> | null = null) {
        if(data != null) {
            this.attraction = new Attraction(data.attraction);
            this.park = new Park(data.park);
        }
    }
}