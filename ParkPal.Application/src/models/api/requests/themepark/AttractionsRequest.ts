import WaitTimeFilter from "@/models/store/WaitTimeFilter";

export default class AttractionsRequest {
    filters?: WaitTimeFilter = new WaitTimeFilter();
    favouriteIds?: Array<string> = [];

    constructor(data: Pick<AttractionsRequest, "filters" | "favouriteIds"> | null = null) {
        if(data != null) {
            this.filters = data.filters;
            this.favouriteIds = data.favouriteIds;
        }
    }
}