ALTER TABLE "Alerts"."LiveActivityMonitor"
    ADD COLUMN "LastSentWaitTime" INTEGER NULL,
    ADD COLUMN "LastSentStatus" INTEGER NULL;