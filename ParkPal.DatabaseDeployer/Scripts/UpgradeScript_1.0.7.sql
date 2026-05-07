-- 1. Drop the foreign key constraint first (so Postgres doesn't block the type change)
ALTER TABLE "Users"."Itinerary"
DROP CONSTRAINT IF EXISTS "FK_Itinerary_AppUser";

-- 2. Change the column type and cast any existing UUID data to TEXT safely
ALTER TABLE "Users"."Itinerary"
ALTER COLUMN "AppUserId" TYPE TEXT USING "AppUserId"::TEXT;