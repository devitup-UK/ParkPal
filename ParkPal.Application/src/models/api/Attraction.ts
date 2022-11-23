import {AttractionStatus} from "@/models/enums/AttractionStatus";
import axios from "axios";

export default class Attraction {
    attractionId?: string;
    name?: string;
    image?: string;
    status?: AttractionStatus;
    thrill?: boolean;
    hidden?: boolean;
    waitTime?: number | null;

    constructor(data: Pick<Attraction, "attractionId" | "name" | "image" | "status" | "thrill" | "hidden" | "waitTime"> | null = null) {
        if(data != null) {
            this.attractionId = data.attractionId;
            this.name = data.name;
            this.image = data.image;
            this.status = data.status;
            this.thrill = data.thrill;
            this.hidden = data.hidden;
            this.waitTime = data.waitTime;
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