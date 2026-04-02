CREATE TABLE IF NOT EXISTS "AspNetRoles" (
                                             "Id"               TEXT NOT NULL,
                                             "Name"             TEXT NULL,
                                             "NormalizedName"   TEXT NULL,
                                             "ConcurrencyStamp" TEXT NULL,
                                             CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id")
    );

CREATE UNIQUE INDEX IF NOT EXISTS "RoleNameIndex"
    ON "AspNetRoles" ("NormalizedName")
    WHERE "NormalizedName" IS NOT NULL;


CREATE TABLE IF NOT EXISTS "AspNetUsers" (
                                             "Id"                   TEXT    NOT NULL,
                                             "FirstName"            TEXT    NOT NULL,
                                             "LastName"             TEXT    NOT NULL,
                                             "DateOfBirth"          TEXT    NOT NULL,  
                                             "Role"                 INTEGER NOT NULL,
                                             "UserName"             TEXT    NULL,
                                             "NormalizedUserName"   TEXT    NULL,
                                             "Email"                TEXT    NULL,
                                             "NormalizedEmail"      TEXT    NULL,
                                             "EmailConfirmed"       INTEGER NOT NULL,
                                             "PasswordHash"         TEXT    NULL,
                                             "SecurityStamp"        TEXT    NULL,
                                             "ConcurrencyStamp"     TEXT    NULL,
                                             "PhoneNumber"          TEXT    NULL,
                                             "PhoneNumberConfirmed" INTEGER NOT NULL,
                                             "TwoFactorEnabled"     INTEGER NOT NULL,
                                             "LockoutEnd"           TEXT    NULL,
                                             "LockoutEnabled"       INTEGER NOT NULL,
                                             "AccessFailedCount"    INTEGER NOT NULL,
                                             CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id")
    );

CREATE INDEX IF NOT EXISTS "EmailIndex"
    ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX IF NOT EXISTS "UserNameIndex"
    ON "AspNetUsers" ("NormalizedUserName")
    WHERE "NormalizedUserName" IS NOT NULL;


CREATE TABLE IF NOT EXISTS "AspNetRoleClaims" (
                                                  "Id"         INTEGER NOT NULL,
                                                  "RoleId"     TEXT    NOT NULL,
                                                  "ClaimType"  TEXT    NULL,
                                                  "ClaimValue" TEXT    NULL,
                                                  CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id" AUTOINCREMENT),
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId"
    FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
    );

CREATE INDEX IF NOT EXISTS "IX_AspNetRoleClaims_RoleId"
    ON "AspNetRoleClaims" ("RoleId");


CREATE TABLE IF NOT EXISTS "AspNetUserClaims" (
                                                  "Id"         INTEGER NOT NULL,
                                                  "UserId"     TEXT    NOT NULL,
                                                  "ClaimType"  TEXT    NULL,
                                                  "ClaimValue" TEXT    NULL,
                                                  CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id" AUTOINCREMENT),
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId"
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
    );

CREATE INDEX IF NOT EXISTS "IX_AspNetUserClaims_UserId"
    ON "AspNetUserClaims" ("UserId");


CREATE TABLE IF NOT EXISTS "AspNetUserLogins" (
                                                  "LoginProvider"       TEXT NOT NULL,
                                                  "ProviderKey"         TEXT NOT NULL,
                                                  "ProviderDisplayName" TEXT NULL,
                                                  "UserId"              TEXT NOT NULL,
                                                  CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId"
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
    );

CREATE INDEX IF NOT EXISTS "IX_AspNetUserLogins_UserId"
    ON "AspNetUserLogins" ("UserId");


CREATE TABLE IF NOT EXISTS "AspNetUserRoles" (
                                                 "UserId" TEXT NOT NULL,
                                                 "RoleId" TEXT NOT NULL,
                                                 CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId"
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId"
    FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
    );

CREATE INDEX IF NOT EXISTS "IX_AspNetUserRoles_RoleId"
    ON "AspNetUserRoles" ("RoleId");


CREATE TABLE IF NOT EXISTS "AspNetUserTokens" (
                                                  "UserId"        TEXT NOT NULL,
                                                  "LoginProvider" TEXT NOT NULL,
                                                  "Name"          TEXT NOT NULL,
                                                  "Value"         TEXT NULL,
                                                  CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId"
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
    );

