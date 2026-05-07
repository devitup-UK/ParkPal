CREATE SCHEMA IF NOT EXISTS "CrowdSource";

-- 1. The Trust Profile (Tracks the Waze-style trust score)
CREATE TABLE IF NOT EXISTS "Users"."Profile" (
                                                 "AppUserId" text PRIMARY KEY,
                                                 "TrustScore" int DEFAULT 0,
                                                 "TotalSubmissions" int DEFAULT 0,
                                                 "FirstSeenAt" timestamp with time zone DEFAULT now()
    );

-- 2. The Quarantine Zone (Where the raw submissions live)
CREATE TABLE IF NOT EXISTS "CrowdSource"."Submission" (
                                                          "SubmissionId" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    "AppUserId" text NOT NULL REFERENCES "Users"."Profile" ("AppUserId"),
    "AttractionId" text NOT NULL,
    "ReportedStatus" int NOT NULL, -- 0 = Operating, 1 = Down, 2 = Closed
    "ReportedWaitTime" int,        -- Null if the ride is reported down!
    "Latitude" double precision,   -- For the geofence check
    "Longitude" double precision,
    "CreatedAt" timestamp with time zone DEFAULT now()
    );

-- 3. Indexes for lightning-fast averaging
-- This helps us quickly calculate "Give me all Operating wait times for Space Mountain in the last hour"
CREATE INDEX IF NOT EXISTS "IX_Submission_Attraction_Time"
    ON "CrowdSource"."Submission" ("AttractionId", "CreatedAt");

-- This helps us enforce the spam filter: "Did this user already submit this ride recently?"
CREATE INDEX IF NOT EXISTS "IX_Submission_User_Attraction_Time"
    ON "CrowdSource"."Submission" ("AppUserId", "AttractionId", "CreatedAt");