CREATE SCHEMA IF NOT EXISTS "Users";
CREATE SCHEMA IF NOT EXISTS "Alerts";

-- 1. The Devices Table
CREATE TABLE IF NOT EXISTS "Users"."Device" (
                                                "DeviceToken" text PRIMARY KEY, -- Apple's push token
                                                "AppUserId" text NOT NULL,      -- The iCloud UUID
                                                "LastActiveAt" timestamp with time zone DEFAULT now()
    );

-- Index so the SyncWorker can quickly find all tokens for a specific user
CREATE INDEX IF NOT EXISTS "IX_Device_AppUserId" ON "Users"."Device" ("AppUserId");

-- 2. The Alerts Table (Now tied to the User, not the phone!)
CREATE TABLE IF NOT EXISTS "Alerts"."WaitTimeAlert" (
                                                        "AlertId" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    "AppUserId" text NOT NULL,
    "AttractionId" text NOT NULL,
    "AlertType" int NOT NULL,
    "TargetWaitTime" int NOT NULL,
    "IsActive" boolean DEFAULT TRUE,
    "CreatedAt" timestamp with time zone DEFAULT now(),
    -- ⭐️ Ensure Abi can only have ONE active alert per ride!
    UNIQUE ("AppUserId", "AttractionId")
    );

CREATE INDEX IF NOT EXISTS "IX_Alerts_Active_Attraction"
    ON "Alerts"."WaitTimeAlert" ("AttractionId")
    WHERE "IsActive" = TRUE;