PRAGMA foreign_keys = ON;

INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp") VALUES
                                                                                   ('C65FC3FC-BFA2-4B6C-A9BF-842184B50B88', 'Professor', 'PROFESSOR', lower(hex(randomblob(16)))),
                                                                                   ('6982247A-6AA6-4CE2-8372-F97791D4854C', 'Assistant', 'ASSISTANT', lower(hex(randomblob(16)))),
                                                                                   ('434D0F7B-4B62-4C5A-9ED2-BDDADCB78D28', 'Student',   'STUDENT',   lower(hex(randomblob(16))));

INSERT INTO "AspNetUsers" (
    "Id", "FirstName", "LastName", "DateOfBirth", "Role",
    "UserName", "NormalizedUserName",
    "Email", "NormalizedEmail", "EmailConfirmed",
    "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
    "PhoneNumber", "PhoneNumberConfirmed",
    "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount"
) VALUES
      ('86EA686E-E4DA-41F0-9D28-9319FE0430DD', 'Alice',  'Johnson', '1975-04-12 00:00:00', 0,
       'alice.johnson', 'ALICE.JOHNSON',
       'alice.johnson@university.edu', 'ALICE.JOHNSON@UNIVERSITY.EDU', 1,
       'AQAAAAIAAYagAAAAEPlaceholderHashAlice==',  lower(hex(randomblob(16))), lower(hex(randomblob(16))),
       NULL, 0, 0, NULL, 0, 0),

      ('91D41D93-B77B-48EA-B333-ABE17DCBA6F5', 'Robert', 'Smith',   '1968-09-30 00:00:00', 0,
       'robert.smith',  'ROBERT.SMITH',
       'robert.smith@university.edu',  'ROBERT.SMITH@UNIVERSITY.EDU',  1,
       'AQAAAAIAAYagAAAAEPlaceholderHashRobert==', lower(hex(randomblob(16))), lower(hex(randomblob(16))),
       NULL, 0, 0, NULL, 0, 0),

      ('58073CE9-7EC2-46EB-9AFE-FB273A005592', 'Maria',  'Kovac',   '1990-06-15 00:00:00', 1,
       'maria.kovac',   'MARIA.KOVAC',
       'maria.kovac@university.edu',   'MARIA.KOVAC@UNIVERSITY.EDU',   1,
       'AQAAAAIAAYagAAAAEPlaceholderHashMaria==',  lower(hex(randomblob(16))), lower(hex(randomblob(16))),
       NULL, 0, 0, NULL, 0, 0),

      ('676AE196-2094-45E7-BC6C-F2483B8AE7B8', 'David',  'Lee',     '1992-11-03 00:00:00', 1,
       'david.lee',     'DAVID.LEE',
       'david.lee@university.edu',     'DAVID.LEE@UNIVERSITY.EDU',     1,
       'AQAAAAIAAYagAAAAEPlaceholderHashDavid==',  lower(hex(randomblob(16))), lower(hex(randomblob(16))),
       NULL, 0, 0, NULL, 0, 0),

      ('C0AE787E-2A34-4081-9F80-5DCB436A8979', 'Emily',  'Brown',   '2001-02-20 00:00:00', 2,
       'emily.brown',   'EMILY.BROWN',
       'emily.brown@student.university.edu',  'EMILY.BROWN@STUDENT.UNIVERSITY.EDU',  1,
       'AQAAAAIAAYagAAAAEPlaceholderHashEmily==',  lower(hex(randomblob(16))), lower(hex(randomblob(16))),
       NULL, 0, 0, NULL, 1, 0),

      ('9BC6BEAB-4C0C-48B8-B538-612E49B29831', 'James',  'Wilson',  '2000-07-08 00:00:00', 2,
       'james.wilson',  'JAMES.WILSON',
       'james.wilson@student.university.edu', 'JAMES.WILSON@STUDENT.UNIVERSITY.EDU', 1,
       'AQAAAAIAAYagAAAAEPlaceholderHashJames==',  lower(hex(randomblob(16))), lower(hex(randomblob(16))),
       NULL, 0, 0, NULL, 1, 0),

      ('604C87C9-E1FC-4FC9-89ED-5298DFAD420F', 'Sofia',  'Petrov',  '2002-03-17 00:00:00', 2,
       'sofia.petrov',  'SOFIA.PETROV',
       'sofia.petrov@student.university.edu', 'SOFIA.PETROV@STUDENT.UNIVERSITY.EDU', 1,
       'AQAAAAIAAYagAAAAEPlaceholderHashSofia==',  lower(hex(randomblob(16))), lower(hex(randomblob(16))),
       NULL, 0, 0, NULL, 1, 0),

      ('E708655B-1994-4940-91A7-07D94F85F159', 'Liam',   'Nguyen',  '2001-12-25 00:00:00', 2,
       'liam.nguyen',   'LIAM.NGUYEN',
       'liam.nguyen@student.university.edu',  'LIAM.NGUYEN@STUDENT.UNIVERSITY.EDU',  1,
       'AQAAAAIAAYagAAAAEPlaceholderHashLiam==',   lower(hex(randomblob(16))), lower(hex(randomblob(16))),
       NULL, 0, 0, NULL, 1, 0);


INSERT INTO "AspNetUserRoles" ("UserId", "RoleId") VALUES
                                                       ('86EA686E-E4DA-41F0-9D28-9319FE0430DD', 'C65FC3FC-BFA2-4B6C-A9BF-842184B50B88'), -- Alice  → Professor
                                                       ('91D41D93-B77B-48EA-B333-ABE17DCBA6F5', 'C65FC3FC-BFA2-4B6C-A9BF-842184B50B88'), -- Robert → Professor
                                                       ('58073CE9-7EC2-46EB-9AFE-FB273A005592', '6982247A-6AA6-4CE2-8372-F97791D4854C'), -- Maria  → Assistant
                                                       ('676AE196-2094-45E7-BC6C-F2483B8AE7B8', '6982247A-6AA6-4CE2-8372-F97791D4854C'), -- David  → Assistant
                                                       ('C0AE787E-2A34-4081-9F80-5DCB436A8979', '434D0F7B-4B62-4C5A-9ED2-BDDADCB78D28'), -- Emily  → Student
                                                       ('9BC6BEAB-4C0C-48B8-B538-612E49B29831', '434D0F7B-4B62-4C5A-9ED2-BDDADCB78D28'), -- James  → Student
                                                       ('604C87C9-E1FC-4FC9-89ED-5298DFAD420F', '434D0F7B-4B62-4C5A-9ED2-BDDADCB78D28'), -- Sofia  → Student
                                                       ('E708655B-1994-4940-91A7-07D94F85F159', '434D0F7B-4B62-4C5A-9ED2-BDDADCB78D28'); -- Liam   → Student


INSERT INTO "Courses" ("Id", "Title", "Credits") VALUES
                                                     ('D3F04A4F-AE7C-45F5-89A6-E4013D0409E4', 'Introduction to Programming',    6.0),
                                                     ('BDFEEC67-2CBC-4461-B76F-131C6F4FA4E1', 'Data Structures and Algorithms', 6.0),
                                                     ('2FF56634-C987-4325-9E2D-A2DC63265EF2', 'Database Systems',               6.0),
                                                     ('A79D7364-3257-4E99-88AE-24F4EE1BA84A', 'Software Engineering',           4.0),
                                                     ('0FC92639-7ACC-4DAC-9A28-9BF12069B29A', 'Computer Networks',              4.0);


INSERT INTO "Teaches" ("Id", "StartDate", "EndDate", "UserId", "CourseId") VALUES
                                                                               ('36E14429-1B58-471A-82CF-72B5BF1896BF', '2024-10-01 00:00:00', '2025-01-31 00:00:00', '86EA686E-E4DA-41F0-9D28-9319FE0430DD', 'D3F04A4F-AE7C-45F5-89A6-E4013D0409E4'), -- Alice  → Intro to Programming
                                                                               ('7A0DF18F-6C04-4E4B-916C-B18A3FB439F8', '2024-10-01 00:00:00', '2025-01-31 00:00:00', '86EA686E-E4DA-41F0-9D28-9319FE0430DD', 'BDFEEC67-2CBC-4461-B76F-131C6F4FA4E1'), -- Alice  → Data Structures
                                                                               ('6EA72B54-9F44-48EF-B1CA-F4AE58EFE0B2', '2024-10-01 00:00:00', '2025-01-31 00:00:00', '91D41D93-B77B-48EA-B333-ABE17DCBA6F5', '2FF56634-C987-4325-9E2D-A2DC63265EF2'), -- Robert → Database Systems
                                                                               ('B1F72F26-7808-456C-8384-39F2F2316E28', '2024-10-01 00:00:00', '2025-01-31 00:00:00', '91D41D93-B77B-48EA-B333-ABE17DCBA6F5', 'A79D7364-3257-4E99-88AE-24F4EE1BA84A'), -- Robert → Software Engineering
                                                                               ('EC0876D6-4136-4987-B780-740420144E2A', '2025-02-01 00:00:00', '2025-06-30 00:00:00', '86EA686E-E4DA-41F0-9D28-9319FE0430DD', '2FF56634-C987-4325-9E2D-A2DC63265EF2'), -- Alice  → Database Systems (summer)
                                                                               ('144C33F1-CAC6-4A64-B085-09C614B36F1F', '2025-02-01 00:00:00', '2025-06-30 00:00:00', '91D41D93-B77B-48EA-B333-ABE17DCBA6F5', '0FC92639-7ACC-4DAC-9A28-9BF12069B29A'), -- Robert → Computer Networks
                                                                               ('216E5AF1-C0A9-44AF-8B72-28364AC37C40', '2024-10-01 00:00:00', '2025-01-31 00:00:00', '58073CE9-7EC2-46EB-9AFE-FB273A005592', 'D3F04A4F-AE7C-45F5-89A6-E4013D0409E4'), -- Maria  → Intro to Programming
                                                                               ('A4A45368-6508-4EFD-9504-21356F59AB05', '2024-10-01 00:00:00', '2025-01-31 00:00:00', '676AE196-2094-45E7-BC6C-F2483B8AE7B8', '2FF56634-C987-4325-9E2D-A2DC63265EF2'); -- David  → Database Systems


INSERT INTO "ExamSlots" ("Id", "StartTime", "EndTime", "SessionType", "CourseId") VALUES
                                                                                      ('2B96EFDB-6E28-415B-9548-1D0DD8ED3E05', '2025-01-15 09:00:00', '2025-01-15 11:00:00', 0, 'D3F04A4F-AE7C-45F5-89A6-E4013D0409E4'),
                                                                                      ('5C16A0A5-18C5-4053-8006-D2E6956357F1', '2025-01-17 09:00:00', '2025-01-17 11:00:00', 0, 'D3F04A4F-AE7C-45F5-89A6-E4013D0409E4'),
                                                                                      ('3055CD33-1803-4646-86A0-AD2DC56EC7E6', '2025-01-20 10:00:00', '2025-01-20 12:00:00', 0, 'BDFEEC67-2CBC-4461-B76F-131C6F4FA4E1'),
                                                                                      ('6E3B466E-3024-4F55-8321-96B64FAC9EA1', '2025-01-22 10:00:00', '2025-01-22 12:00:00', 0, '2FF56634-C987-4325-9E2D-A2DC63265EF2'),
                                                                                      ('320CBDDB-5976-40C6-BC30-17417470F768', '2025-01-24 11:00:00', '2025-01-24 13:00:00', 0, 'A79D7364-3257-4E99-88AE-24F4EE1BA84A'),
                                                                                      ('47093A91-BB07-4648-BA04-6AFFB5E3DCA9', '2025-06-10 09:00:00', '2025-06-10 11:00:00', 1, 'D3F04A4F-AE7C-45F5-89A6-E4013D0409E4'),
                                                                                      ('5051E328-0C73-4EF1-BF6B-089F683537D4', '2025-06-12 09:00:00', '2025-06-12 11:00:00', 1, 'BDFEEC67-2CBC-4461-B76F-131C6F4FA4E1'),
                                                                                      ('DB72FBB0-6745-48D0-9910-A78F3BF56BE1', '2025-06-14 10:00:00', '2025-06-14 12:00:00', 1, '2FF56634-C987-4325-9E2D-A2DC63265EF2'),
                                                                                      ('518FC460-E820-4304-9E90-365AD6AD8089', '2025-06-16 10:00:00', '2025-06-16 12:00:00', 1, '0FC92639-7ACC-4DAC-9A28-9BF12069B29A'),
                                                                                      ('5BD1F524-60F2-4883-B36A-18DACF4D3D7C', '2025-09-05 09:00:00', '2025-09-05 11:00:00', 2, 'D3F04A4F-AE7C-45F5-89A6-E4013D0409E4'),
                                                                                      ('8EC21551-C0C6-4F16-A3BF-E252A96CD1E7', '2025-09-08 10:00:00', '2025-09-08 12:00:00', 2, 'A79D7364-3257-4E99-88AE-24F4EE1BA84A');

INSERT INTO "ExamAttempts" ("Id", "Grade", "UserId", "ExamSlotId", "CourseId") VALUES
                                                                                   ('AF78D8C1-7CC4-4D5D-9F73-25B972BFE037', 8,  'C0AE787E-2A34-4081-9F80-5DCB436A8979', '2B96EFDB-6E28-415B-9548-1D0DD8ED3E05', 'D3F04A4F-AE7C-45F5-89A6-E4013D0409E4'),
                                                                                   ('1D91F1C5-6567-4BB9-9A0C-A37E9963AC40', 7,  'C0AE787E-2A34-4081-9F80-5DCB436A8979', '3055CD33-1803-4646-86A0-AD2DC56EC7E6', 'BDFEEC67-2CBC-4461-B76F-131C6F4FA4E1'),
                                                                                   ('98F8DB6B-279D-46E9-BD54-644F525CD0B8', 9,  'C0AE787E-2A34-4081-9F80-5DCB436A8979', '6E3B466E-3024-4F55-8321-96B64FAC9EA1', '2FF56634-C987-4325-9E2D-A2DC63265EF2'),
                                                                                   ('5B9466EA-C398-4814-96D7-64B20618659A', 6,  '9BC6BEAB-4C0C-48B8-B538-612E49B29831', '2B96EFDB-6E28-415B-9548-1D0DD8ED3E05', 'D3F04A4F-AE7C-45F5-89A6-E4013D0409E4'),
                                                                                   ('44A1CD5C-694D-446C-A795-39F139BF3C04', 5,  '9BC6BEAB-4C0C-48B8-B538-612E49B29831', '3055CD33-1803-4646-86A0-AD2DC56EC7E6', 'BDFEEC67-2CBC-4461-B76F-131C6F4FA4E1'), -- failed
                                                                                   ('3AD73EB2-65C2-4E9D-9C9A-5C87C33F381A', 7,  '9BC6BEAB-4C0C-48B8-B538-612E49B29831', '5051E328-0C73-4EF1-BF6B-089F683537D4', 'BDFEEC67-2CBC-4461-B76F-131C6F4FA4E1'), -- resit
                                                                                   ('CDD11E8C-4271-4974-8ADD-CE23D068E0B5', 10, '604C87C9-E1FC-4FC9-89ED-5298DFAD420F', '5C16A0A5-18C5-4053-8006-D2E6956357F1', 'D3F04A4F-AE7C-45F5-89A6-E4013D0409E4'),
                                                                                   ('C4A9C221-C2B5-4511-8AF1-C602FCF129BD', 9,  '604C87C9-E1FC-4FC9-89ED-5298DFAD420F', '6E3B466E-3024-4F55-8321-96B64FAC9EA1', '2FF56634-C987-4325-9E2D-A2DC63265EF2'),
                                                                                   ('42229137-C5C6-4F69-A545-746C0F770459', 8,  '604C87C9-E1FC-4FC9-89ED-5298DFAD420F', '320CBDDB-5976-40C6-BC30-17417470F768', 'A79D7364-3257-4E99-88AE-24F4EE1BA84A'),
                                                                                   ('4458B834-FF28-4369-8057-548DBA4BB505', 7,  'E708655B-1994-4940-91A7-07D94F85F159', '2B96EFDB-6E28-415B-9548-1D0DD8ED3E05', 'D3F04A4F-AE7C-45F5-89A6-E4013D0409E4'),
                                                                                   ('98DE6FFA-B259-454C-9AA7-C32AAC2F9A50', 6,  'E708655B-1994-4940-91A7-07D94F85F159', '320CBDDB-5976-40C6-BC30-17417470F768', 'A79D7364-3257-4E99-88AE-24F4EE1BA84A'),
                                                                                   ('EF43973D-772A-49FC-8E9A-2C3BB63CBACA', 8,  'E708655B-1994-4940-91A7-07D94F85F159', '518FC460-E820-4304-9E90-365AD6AD8089', '0FC92639-7ACC-4DAC-9A28-9BF12069B29A');