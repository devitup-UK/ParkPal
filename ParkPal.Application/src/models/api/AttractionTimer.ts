export default class AttractionTimer {
    attractionTimerId?: number;
    minuteInterval?: number;
    parkId?: string;
    attractionId?: string;
    criteriaType?: number;
    waitTime = 0;
    enabled?: boolean;

    constructor(data: Pick<AttractionTimer, "attractionTimerId" | "minuteInterval" | "parkId" | "attractionId" | "criteriaType" | "waitTime" | "enabled"> | null = null) {
        if(data != null) {
            this.attractionTimerId = data.attractionTimerId;
            this.minuteInterval = data.minuteInterval;
            this.parkId = data.parkId;
            this.attractionId = data.attractionId;
            this.criteriaType = data.criteriaType;
            this.waitTime = data.waitTime;
            this.enabled = data.enabled;
        }
    }
}