-- =========================================================================
-- Upgrade Script 1.0.26
-- Description: Creates the ItineraryMember junction table for Shared Plans
-- =========================================================================

-- 1. Create the Junction Table
CREATE TABLE IF NOT EXISTS "Users"."ItineraryMember" (
    "ItineraryId" uuid NOT NULL,
    "AppUserId" character varying(255) NOT NULL,
    "JoinedAt" timestamp with time zone NOT NULL DEFAULT NOW(),

    -- ⭐️ 1. Composite Primary Key: Prevents duplicate joins!
    CONSTRAINT "PK_ItineraryMember" PRIMARY KEY ("ItineraryId", "AppUserId"),

    -- ⭐️ 2. Foreign Key with CASCADE: If the main plan is deleted, delete the memberships!
    CONSTRAINT "FK_ItineraryMember_Itinerary" FOREIGN KEY ("ItineraryId")
    REFERENCES "Users"."Itinerary" ("Id") ON DELETE CASCADE
    );

-- 2. Create an Index for the AppUserId
-- ⭐️ 3. Performance Win: Because your GetUserPlansAsync query looks up memberships 
-- by AppUserId, this index stops Postgres from scanning the whole table!
CREATE INDEX IF NOT EXISTS "IX_ItineraryMember_AppUserId"
    ON "Users"."ItineraryMember" ("AppUserId");