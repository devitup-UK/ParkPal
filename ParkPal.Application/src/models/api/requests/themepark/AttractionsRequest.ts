import WaitTimeFilter from "@/models/store/WaitTimeFilter";

export default class AttractionsRequest {
    filters?: WaitTimeFilter = new WaitTimeFilter();
    favouriteAttractionIds?: Array<string> = [];

    constructor(data: Pick<AttractionsRequest, "filters" | "favouriteAttractionIds"> | null = null) {
        if(data != null) {
            this.filters = data.filters;
            this.favouriteAttractionIds = data.favouriteAttractionIds;
        }
    }
}