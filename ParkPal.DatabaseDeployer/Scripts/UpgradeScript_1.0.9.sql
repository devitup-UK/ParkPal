CREATE TABLE "Alerts"."LiveActivityMonitor" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "AppUserId" text NOT NULL,
    "AttractionId" text NOT NULL,
    "PushToken" VARCHAR(255) NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "ExpiresAt" TIMESTAMP WITH TIME ZONE NOT NULL,

    -- ⭐️ Crucial: Prevent duplicate monitors for the same user/ride combo!
    CONSTRAINT "UQ_User_Attraction" UNIQUE ("AppUserId", "AttractionId")
);