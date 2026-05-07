-- 1. Create the logical containers
CREATE SCHEMA IF NOT EXISTS "Static";
CREATE SCHEMA IF NOT EXISTS "Live";
CREATE SCHEMA IF NOT EXISTS "History"; -- ⭐️ Fixed schema name

-- 2. STATIC DATA: The "Blueprint" of the World
CREATE TABLE IF NOT EXISTS "Static"."Destination" (
                                                      "DestinationId" VARCHAR(255) PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "City" VARCHAR(255),
    "Country" VARCHAR(255)
    );

CREATE TABLE IF NOT EXISTS "Static"."Park" (
                                               "ParkId" VARCHAR(255) PRIMARY KEY,
    "DestinationId" VARCHAR(255) NOT NULL REFERENCES "Static"."Destination"("DestinationId"),
    "Name" VARCHAR(255) NOT NULL
    );

CREATE TABLE IF NOT EXISTS "Static"."Attraction" (
                                                     "AttractionId" VARCHAR(255) PRIMARY KEY,
    "ParkId" VARCHAR(255) NOT NULL REFERENCES "Static"."Park"("ParkId"),
    "Name" VARCHAR(255) NOT NULL,
    "IsThrill" BOOLEAN NOT NULL DEFAULT FALSE,
    "IsHidden" BOOLEAN NOT NULL DEFAULT FALSE
    );

-- 3. LIVE DATA: The "Right Now" State for the iOS UI
CREATE TABLE IF NOT EXISTS "Live"."AttractionState" (
                                                        "AttractionId" VARCHAR(255) PRIMARY KEY REFERENCES "Static"."Attraction"("AttractionId"),
    "WaitTime" INTEGER,
    "Status" INTEGER NOT NULL,
    "LastUpdated" TIMESTAMP WITH TIME ZONE NOT NULL,
    "SingleRiderWaitTime" INTEGER,
    "LightningLaneReturnStart" TIMESTAMP WITH TIME ZONE,
                                             "LightningLanePrice" DECIMAL(10, 2),
    "IsVirtualQueueOnly" BOOLEAN NOT NULL DEFAULT FALSE
    );

-- 4. HISTORY DATA: The "Goldmine" for your Charts
-- ⭐️ Fixed naming to "History"."Attraction"
CREATE TABLE IF NOT EXISTS "History"."Attraction" (
                                                      "HistoryId" SERIAL PRIMARY KEY,
                                                      "AttractionId" VARCHAR(255) NOT NULL REFERENCES "Static"."Attraction"("AttractionId"),
    "WaitTime" INTEGER NOT NULL,
    "Status" INTEGER NOT NULL,
    "StartTime" TIMESTAMP WITH TIME ZONE NOT NULL,
    "LastSeenTime" TIMESTAMP WITH TIME ZONE NOT NULL,
                                 "RawData" JSONB NOT NULL
                                 );

-- Indices for performance
CREATE INDEX IF NOT EXISTS "IX_History_Attraction_Lookup" ON "History"."Attraction" ("AttractionId", "StartTime" DESC);
CREATE INDEX IF NOT EXISTS "IX_Park_Lookup" ON "Static"."Park" ("DestinationId");
CREATE INDEX IF NOT EXISTS "IX_Attraction_Lookup" ON "Static"."Attraction" ("ParkId");