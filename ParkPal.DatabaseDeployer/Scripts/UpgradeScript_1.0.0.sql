-- UpgradeScript_1.0.0.dbu (The Definitive ParkPal Postgres Baseline)
-- Includes: Base Setup, APNs Refactor, Park-Wide Alerts, & ParkPal+ Vouchers

-- ==========================================
-- 1. SCHEMAS
-- ==========================================
CREATE SCHEMA IF NOT EXISTS "Device";
CREATE SCHEMA IF NOT EXISTS "Notification";
CREATE SCHEMA IF NOT EXISTS "Subscription";

-- ==========================================
-- 2. DEVICES (Replaces OneSignal)
-- ==========================================
CREATE TABLE IF NOT EXISTS "Device"."Token" (
                                                "TokenId" SERIAL PRIMARY KEY,
                                                "ApnsToken" VARCHAR(255) NOT NULL UNIQUE,
    "RegisteredAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
    );

-- ==========================================
-- 3. NOTIFICATION LOOKUP TABLES
-- ==========================================
-- Alert Types (1 = Attraction, 2 = Park)
CREATE TABLE IF NOT EXISTS "Notification"."Type" (
                                                     "TypeId" SERIAL PRIMARY KEY,
                                                     "Name" VARCHAR(50) NOT NULL
    );

INSERT INTO "Notification"."Type" ("TypeId", "Name")
VALUES (1, 'Attraction'), (2, 'Park')
    ON CONFLICT ("TypeId") DO NOTHING;

-- Criteria Types (1 = Less Than, 2 = More Than, 3 = Equal To)
CREATE TABLE IF NOT EXISTS "Notification"."CriteriaType" (
                                                             "CriteriaTypeId" SERIAL PRIMARY KEY,
                                                             "Name" VARCHAR(50) NOT NULL
    );

INSERT INTO "Notification"."CriteriaType" ("CriteriaTypeId", "Name")
VALUES (1, 'Less Than'), (2, 'More Than'), (3, 'Equal To')
    ON CONFLICT ("CriteriaTypeId") DO NOTHING;

-- ==========================================
-- 4. ALERTS (Merges your 1.0.2 'Item' and 1.0.3 logic)
-- ==========================================
CREATE TABLE IF NOT EXISTS "Notification"."Alert" (
                                                      "AlertId" SERIAL PRIMARY KEY,
                                                      "TokenId" INT NOT NULL REFERENCES "Device"."Token"("TokenId") ON DELETE CASCADE,
    "TypeId" INT NOT NULL REFERENCES "Notification"."Type"("TypeId"),
    "CriteriaTypeId" INT NOT NULL REFERENCES "Notification"."CriteriaType"("CriteriaTypeId"),
    "ParkId" VARCHAR(255) NOT NULL,   -- Every alert belongs to a park
    "AttractionId" VARCHAR(255),      -- Nullable for Park-Wide alerts!
    "ThresholdWaitTime" INT NOT NULL DEFAULT 20,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    -- ⭐️ Safety Net: If it's an Attraction alert (TypeId=1), the AttractionId CANNOT be null
    CONSTRAINT "CHK_Alert_Target" CHECK ("TypeId" = 2 OR "AttractionId" IS NOT NULL)
    );

-- Indexes for lightning-fast background worker polling
CREATE INDEX IF NOT EXISTS "IX_Alert_Active_Park" ON "Notification"."Alert" ("ParkId", "IsActive");

-- ==========================================
-- 5. PARKPAL+ SUBSCRIPTIONS (From your 1.0.4)
-- ==========================================
CREATE TABLE IF NOT EXISTS "Subscription"."Voucher" (
                                                        "VoucherId" SERIAL PRIMARY KEY,
                                                        "Code" VARCHAR(255) NOT NULL UNIQUE, -- Added Unique to prevent duplicate codes!
    "Redeemed" BOOLEAN NOT NULL DEFAULT FALSE,
    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
    );

-- ==========================================
-- 6. UPGRADE SERVICE TRACKER
-- ==========================================
CREATE TABLE IF NOT EXISTS "Version" (
                                         "VersionID" SERIAL PRIMARY KEY,
                                         "Major" INT NOT NULL,
                                         "Minor" INT NOT NULL,
                                         "Revision" INT NOT NULL,
                                         "Created" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);