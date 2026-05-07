ALTER TABLE "Users"."ItineraryItem"
ALTER COLUMN "AttractionId" TYPE TEXT USING "AttractionId"::TEXT;