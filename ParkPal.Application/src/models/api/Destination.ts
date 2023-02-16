import Park from "@/models/api/Park";
import axios from "axios";

export default class Destination {
    destinationId = '';
    name = '';
    image?: string;
    location?: string;
    parks: Array<Park> = new Array<Park>();
    hidden?: boolean;
    defaultOrder?: number;
    visible = !this.hidden;


    constructor(data: Pick<Destination, "destinationId" | "name" | "image" | "location" | "parks" | "hidden" | "defaultOrder"> | null = null) {
        if(data != null) {
            this.destinationId = data.destinationId;
            this.name = data.name;
            this.image = data.image;
            this.location = data.location;
            this.hidden = data.hidden;
            this.defaultOrder = data.defaultOrder;
            this.visible = !data.hidden;

            data.parks?.forEach((park: Park) => {
                this.parks.push(new Park(park))
            });
        }
    }

    checkImageExists() {
        return new Promise((resolve) => {
            if(this.image) {
                axios.get('/img/' + this.image).then(() => {
                    resolve(true);
                }).catch(() => {
                    resolve(false);
                });
            }else{
                resolve(true);
            }
        })
    }
}