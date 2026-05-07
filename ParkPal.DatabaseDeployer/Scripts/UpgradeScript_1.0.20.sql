CREATE TABLE IF NOT EXISTS "History"."DailyShowSchedule" (
     "ScheduleId" SERIAL PRIMARY KEY,
     "AttractionId" VARCHAR(255) NOT NULL REFERENCES "Static"."Attraction"("AttractionId"),
    "Date" DATE NOT NULL,
    "Showtimes" JSONB NOT NULL,
    UNIQUE("AttractionId", "Date")
    );

-- (Optional but highly recommended) Add an index to make our "Fallback Query" lightning fast!
CREATE INDEX IF NOT EXISTS "IX_DailyShowSchedule_AttractionId_Date"
    ON "History"."DailyShowSchedule" ("AttractionId", "Date" DESC);