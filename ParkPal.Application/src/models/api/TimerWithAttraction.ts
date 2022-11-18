import AttractionTimer from "@/models/api/AttractionTimer";
import Attraction from "@/models/api/Attraction";
import Park from "@/models/api/Park";

export default class TimerWithAttraction {
    timer?: AttractionTimer;
    attraction?: Attraction;
    park?: Park;

    constructor(data: Pick<TimerWithAttraction, "timer" | "attraction" | "park"> | null = null) {
        if(data != null) {
            this.timer = new AttractionTimer(data.timer);
            this.attraction = new Attraction(data.attraction);
            this.park = new Park(data.park);
        }
    }
}