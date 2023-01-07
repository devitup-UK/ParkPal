import Attraction from "@/models/api/Attraction";
import Park from "@/models/api/Park";
import {NotificationType} from "@/models/enums/NotificationType";


export default class NotificationHoldingArea {
    attraction: Attraction | null = new Attraction();
    park: Park = new Park();
    type: NotificationType = NotificationType.Attraction;

    constructor(data: Pick<NotificationHoldingArea, "attraction" | "park" | "type"> | null = null) {
        if(data != null) {
            this.attraction = new Attraction(data.attraction);
            this.park = new Park(data.park);
            this.type = data.type;
        }
    }
}