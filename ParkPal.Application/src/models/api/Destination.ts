import Park from "@/models/api/Park";
import axios from "axios";

export default class Destination {
    destinationId?: string;
    name?: string;
    image?: string;
    location?: string;
    parks: Array<Park> = new Array<Park>();
    hidden?: boolean;


    constructor(data: Pick<Destination, "destinationId" | "name" | "image" | "location" | "parks" | "hidden"> | null = null) {
        if(data != null) {
            this.destinationId = data.destinationId;
            this.name = data.name;
            this.image = data.image;
            this.location = data.location;
            this.hidden = data.hidden;

            data.parks?.forEach((park: Park) => {
                this.parks.push(new Park(park))
            });
        }
    }

    checkImageExists() {
        return new Promise((resolve, reject) => {
            if(this.image) {
                // console.log('Image', this.image);
                axios.get('/img/' + this.image).then((response) => {
                    // console.log('Response', response);
                    resolve(true);
                }).catch((error) => {
                    // console.log('Error', error);
                    resolve(false);
                });
            }else{
                resolve(true);
            }
        })
    }
}