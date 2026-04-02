CREATE TABLE IF NOT EXISTS "Courses" (
                                         "Id"      TEXT    NOT NULL,
                                         "Title"   TEXT    NOT NULL,
                                         "Credits" NUMERIC NOT NULL,
                                         CONSTRAINT "PK_Courses" PRIMARY KEY ("Id")
    );


CREATE TABLE IF NOT EXISTS "ExamSlots" (
                                           "Id"          TEXT    NOT NULL,
                                           "StartTime"   TEXT    NOT NULL,
                                           "EndTime"     TEXT    NOT NULL,
                                           "SessionType" INTEGER NOT NULL,
                                           "CourseId"    TEXT    NOT NULL,
                                           CONSTRAINT "PK_ExamSlots" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ExamSlots_Courses_CourseId"
    FOREIGN KEY ("CourseId") REFERENCES "Courses" ("Id") ON DELETE CASCADE
    );

CREATE INDEX IF NOT EXISTS "IX_ExamSlots_CourseId"
    ON "ExamSlots" ("CourseId");


CREATE TABLE IF NOT EXISTS "Teaches" (
                                         "Id"        TEXT    NOT NULL,
                                         "StartDate" TEXT    NOT NULL,
                                         "EndDate"   TEXT    NOT NULL,
                                         "UserId"    TEXT    NOT NULL,
                                         "CourseId"  TEXT    NOT NULL,
                                         CONSTRAINT "PK_Teaches" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Teaches_AspNetUsers_UserId"
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Teaches_Courses_CourseId"
    FOREIGN KEY ("CourseId") REFERENCES "Courses" ("Id") ON DELETE CASCADE
    );

CREATE INDEX IF NOT EXISTS "IX_Teaches_UserId"
    ON "Teaches" ("UserId");

CREATE INDEX IF NOT EXISTS "IX_Teaches_CourseId"
    ON "Teaches" ("CourseId");

CREATE TABLE IF NOT EXISTS "ExamAttempts" (
                                              "Id"         TEXT    NOT NULL,
                                              "Grade"      INTEGER NOT NULL,
                                              "UserId"     TEXT    NOT NULL,
                                              "ExamSlotId" TEXT    NOT NULL,
                                              "CourseId"   TEXT    NOT NULL,
                                              CONSTRAINT "PK_ExamAttempts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ExamAttempts_AspNetUsers_UserId"
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE NO ACTION,
    CONSTRAINT "FK_ExamAttempts_ExamSlots_ExamSlotId"
    FOREIGN KEY ("ExamSlotId") REFERENCES "ExamSlots" ("Id") ON DELETE NO ACTION,
    CONSTRAINT "FK_ExamAttempts_Courses_CourseId"
    FOREIGN KEY ("CourseId") REFERENCES "Courses" ("Id") ON DELETE NO ACTION
    );

CREATE INDEX IF NOT EXISTS "IX_ExamAttempts_UserId"
    ON "ExamAttempts" ("UserId");

CREATE INDEX IF NOT EXISTS "IX_ExamAttempts_ExamSlotId"
    ON "ExamAttempts" ("ExamSlotId");

CREATE INDEX IF NOT EXISTS "IX_ExamAttempts_CourseId"
    ON "ExamAttempts" ("CourseId");