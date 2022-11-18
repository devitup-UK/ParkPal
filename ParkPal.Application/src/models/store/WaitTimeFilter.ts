import {WaitTimeFilterType} from "@/models/enums/WaitTimeFilterType";
import {WaitTimeFilterSort} from "@/models/enums/WaitTimeFilterSort";

export default class WaitTimeFilter {
    type: WaitTimeFilterType = WaitTimeFilterType.AllAttractions;
    sort: WaitTimeFilterSort = WaitTimeFilterSort.HighestWaitTime;

    constructor(data: Pick<WaitTimeFilter, "type" | "sort"> | null = null) {
        if(data != null) {
            this.type = data.type;
            this.sort = data.sort;
        }

    }
}