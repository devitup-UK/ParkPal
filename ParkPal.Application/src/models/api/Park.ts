import Attraction from "@/models/api/Attraction";
import axios from "axios";

export default class Park {
    parkId = '';
    name = '';
    image?: string;
    attractions: Array<Attraction> = new Array<Attraction>();
    hidden?: boolean;


    constructor(data: Pick<Park, "parkId" | "name" | "image" | "attractions" | "hidden"> | null = null) {
        if(data != null) {
            this.parkId = data.parkId;
            this.name = data.name;
            this.image = data.image;
            this.hidden = data.hidden;

            data.attractions?.forEach((attraction: Attraction) => {
                this.attractions.push(new Attraction(attraction))
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