CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE "Devices" (
    "DeviceId" serial NOT NULL,
    "Name" character varying(256) NOT NULL,
    "Description" character varying(2000) NOT NULL,
    "DisplayOrder" integer NOT NULL,
    "DateCreated" timestamp with time zone NOT NULL,
    "IsReadOnly" boolean NOT NULL,
    CONSTRAINT "PK_Devices" PRIMARY KEY ("DeviceId")
);

CREATE TABLE "AlternateDeviceIds" (
    "AlternateId" serial NOT NULL,
    "DeviceId" integer NOT NULL,
    "DateCreated" timestamp with time zone NOT NULL,
    "Uuid" uuid NULL,
    "MacAddress" bigint NULL,
    "Manufacture" text NULL,
    "ManufactureId" text NULL,
    CONSTRAINT "PK_AlternateDeviceIds" PRIMARY KEY ("AlternateId"),
    CONSTRAINT "FK_AlternateDeviceIds_Devices_DeviceId" FOREIGN KEY ("DeviceId") REFERENCES "Devices" ("DeviceId") ON DELETE CASCADE
);

CREATE INDEX "IX_AlternateDeviceIds_DeviceId" ON "AlternateDeviceIds" ("DeviceId");

CREATE UNIQUE INDEX "IX_AlternateDeviceIds_MacAddress" ON "AlternateDeviceIds" ("MacAddress");

CREATE UNIQUE INDEX "IX_AlternateDeviceIds_Uuid" ON "AlternateDeviceIds" ("Uuid");

CREATE UNIQUE INDEX "IX_AlternateDeviceIds_Manufacture_ManufactureId" ON "AlternateDeviceIds" ("Manufacture", "ManufactureId");

CREATE INDEX "IX_Devices_DisplayOrder" ON "Devices" ("DisplayOrder");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20190520043135_Initial', '2.2.4-servicing-10062');


