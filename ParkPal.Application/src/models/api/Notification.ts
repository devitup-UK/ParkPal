import NotificationProperties from "@/models/api/NotificationProperties";
import Attraction from "@/models/api/Attraction";
import Park from "@/models/api/Park";

export default class Notification {
    properties: NotificationProperties = new NotificationProperties();
    attraction: Attraction = new Attraction();
    park: Park = new Park();

    constructor(data: Pick<Notification, "properties" | "attraction" | "park"> | null = null) {
        if(data != null) {
            this.properties = new NotificationProperties(data.properties);
            this.attraction = new Attraction(data.attraction);
            this.park = new Park(data.park);
        }
    }
}