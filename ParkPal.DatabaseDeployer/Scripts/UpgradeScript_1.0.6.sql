-- 1. The Parent Itinerary Table
CREATE TABLE "Users"."Itinerary" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "AppUserId" UUID NOT NULL, -- Links to your existing user table
    "Title" VARCHAR(255) NOT NULL, -- e.g., "Abi's Birthday Magic"
    "TripDate" DATE NOT NULL,
    "DestinationName" VARCHAR(255) NOT NULL,
    "ParkName" VARCHAR(255) NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Index for quick lookups when the user opens the "Plan My Day" tab
CREATE INDEX "IX_Itinerary_AppUserId" ON "Users"."Itinerary" ("AppUserId");

-- 2. The Child Items Table
CREATE TABLE "Users"."ItineraryItem" (
     "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
     "ItineraryId" UUID NOT NULL,
    -- Can be 'Attraction', 'Dining', 'Break', etc.
    "ItemType" VARCHAR(50) NOT NULL DEFAULT 'Attraction',
    -- The actual time they need to be there
    "ScheduledTime" TIME NOT NULL,
    -- Nullable: Only populated if it's an actual ride
    "AttractionId" UUID NULL,
    -- Nullable: Used for things like "Lunch Break" or "Grab a Dole Whip!"
    "CustomTitle" VARCHAR(255) NULL,
    "CustomSubtitle" VARCHAR(255) NULL,
    -- The predicted wait time the C# algorithm generated
    "ProjectedWaitTime" INTEGER NULL,
    CONSTRAINT "FK_ItineraryItem_Itinerary" FOREIGN KEY ("ItineraryId")
    REFERENCES "Users"."Itinerary" ("Id") ON DELETE CASCADE
);

-- Index to quickly load the items for a specific plan, sorted by time
CREATE INDEX "IX_ItineraryItem_ItineraryId_Time" ON "Users"."ItineraryItem" ("ItineraryId", "ScheduledTime");