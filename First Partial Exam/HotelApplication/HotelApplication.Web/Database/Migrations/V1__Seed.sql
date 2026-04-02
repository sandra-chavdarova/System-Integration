INSERT INTO "AspNetUsers" (
    "Id", "Discriminator","FirstName", "LastName",
    "UserName", "NormalizedUserName",
    "Email", "NormalizedEmail", "EmailConfirmed",
    "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
    "PhoneNumber", "PhoneNumberConfirmed",
    "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount"
) VALUES
      ('86EA686E-E4DA-41F0-9D28-9319FE0430DD','HotelApplicationUser', 'Alice',  'Johnson',  
       'alice.johnson', 'ALICE.JOHNSON',
       'alice.johnson@university.edu', 'ALICE.JOHNSON@UNIVERSITY.EDU', 1,
       'AQAAAAIAAYagAAAAEPlaceholderHashAlice==',  lower(hex(randomblob(16))), lower(hex(randomblob(16))),
       NULL, 0, 0, NULL, 0, 0),
      
      ('91D41D93-B77B-48EA-B333-ABE17DCBA6F5', 'HotelApplicationUser','Robert', 'Smith',  
       'robert.smith',  'ROBERT.SMITH',
       'robert.smith@university.edu',  'ROBERT.SMITH@UNIVERSITY.EDU',  1,
       'AQAAAAIAAYagAAAAEPlaceholderHashRobert==', lower(hex(randomblob(16))), lower(hex(randomblob(16))),
       NULL, 0, 0, NULL, 0, 0),

      ('58073CE9-7EC2-46EB-9AFE-FB273A005592', 'HotelApplicationUser','Maria',  'Kovac',
       'maria.kovac',   'MARIA.KOVAC',
       'maria.kovac@university.edu',   'MARIA.KOVAC@UNIVERSITY.EDU',   1,
       'AQAAAAIAAYagAAAAEPlaceholderHashMaria==',  lower(hex(randomblob(16))), lower(hex(randomblob(16))),
       NULL, 0, 0, NULL, 0, 0),

      ('676AE196-2094-45E7-BC6C-F2483B8AE7B8','HotelApplicationUser', 'David',  'Lee',      
       'david.lee',     'DAVID.LEE',
       'david.lee@university.edu',     'DAVID.LEE@UNIVERSITY.EDU',     1,
       'AQAAAAIAAYagAAAAEPlaceholderHashDavid==',  lower(hex(randomblob(16))), lower(hex(randomblob(16))),
       NULL, 0, 0, NULL, 0, 0),

      ('C0AE787E-2A34-4081-9F80-5DCB436A8979', 'HotelApplicationUser','Emily',  'Brown',   
       'emily.brown',   'EMILY.BROWN',
       'emily.brown@student.university.edu',  'EMILY.BROWN@STUDENT.UNIVERSITY.EDU',  1,
       'AQAAAAIAAYagAAAAEPlaceholderHashEmily==',  lower(hex(randomblob(16))), lower(hex(randomblob(16))),
       NULL, 0, 0, NULL, 1, 0),

      ('9BC6BEAB-4C0C-48B8-B538-612E49B29831', 'HotelApplicationUser','James',  'Wilson',  
       'james.wilson',  'JAMES.WILSON',
       'james.wilson@student.university.edu', 'JAMES.WILSON@STUDENT.UNIVERSITY.EDU', 1,
       'AQAAAAIAAYagAAAAEPlaceholderHashJames==',  lower(hex(randomblob(16))), lower(hex(randomblob(16))),
       NULL, 0, 0, NULL, 1, 0),

      ('604C87C9-E1FC-4FC9-89ED-5298DFAD420F', 'HotelApplicationUser','Sofia',  'Petrov',  
       'sofia.petrov',  'SOFIA.PETROV',
       'sofia.petrov@student.university.edu', 'SOFIA.PETROV@STUDENT.UNIVERSITY.EDU', 1,
       'AQAAAAIAAYagAAAAEPlaceholderHashSofia==',  lower(hex(randomblob(16))), lower(hex(randomblob(16))),
       NULL, 0, 0, NULL, 1, 0),

      ('E708655B-1994-4940-91A7-07D94F85F159', 'HotelApplicationUser','Liam',   'Nguyen',  
       'liam.nguyen',   'LIAM.NGUYEN',
       'liam.nguyen@student.university.edu',  'LIAM.NGUYEN@STUDENT.UNIVERSITY.EDU',  1,
       'AQAAAAIAAYagAAAAEPlaceholderHashLiam==',   lower(hex(randomblob(16))), lower(hex(randomblob(16))),
       NULL, 0, 0, NULL, 1, 0);






INSERT INTO "Hotels" ("Id", "Name", "Address") VALUES
('11111111-1111-1111-1111-111111111111', 'Grand Palace Hotel', '123 Main Street, Skopje'),
('11111111-1111-1111-1111-111111111112', 'Alexander Palace Hotel', 'Street 5, Skopje'),
('11111111-1111-1111-1111-111111111113', 'Golden Hotel', '5th Avenue, NYC');


INSERT INTO "Rooms" ("Id", "Status", "Capacity", "RoomNumber", "PricePerNight", "HotelId") VALUES
                                                                                               ('22222222-2222-2222-2222-222222222221', 0, 2, 101, 50.0, '11111111-1111-1111-1111-111111111111'),
                                                                                               ('22222222-2222-2222-2222-222222222222', 0, 4, 102, 80.0, '11111111-1111-1111-1111-111111111112'),
                                                                                               ('22222222-2222-2222-2222-222222222223', 2, 3, 206, 60.0, '11111111-1111-1111-1111-111111111113'),
                                                                                               ('22222222-2222-2222-2222-222222222224', 0, 4, 305, 80.0, '11111111-1111-1111-1111-111111111112');

INSERT INTO "HotelServices" ("Id", "Type", "Description", "Price", "Duration", "HotelId") VALUES
                                                                                              ('44444444-4444-4444-4444-444444444444', 0, 'Spa & Wellness package', 100.0, 120, '11111111-1111-1111-1111-111111111111'),
                                                                                              ('44444444-4444-4444-4444-444444444445', 1, 'Light breakfast', 30.0, 30, '11111111-1111-1111-1111-111111111111'),
                                                                                              ('44444444-4444-4444-4444-444444444446', 2, 'Head Relax Massage', 40.0, 40, '11111111-1111-1111-1111-111111111111'),
                                                                                              ('44444444-4444-4444-4444-444444444447', 1, 'Breakfast Buffet', 50.0, 30, '11111111-1111-1111-1111-111111111112'),
                                                                                              ('44444444-4444-4444-4444-444444444448', 0, 'Indoor pool', 15.0, 180, '11111111-1111-1111-1111-111111111112'),
                                                                                              ('44444444-4444-4444-4444-444444444449', 2, 'Full Body Massage', 60.0, 60, '11111111-1111-1111-1111-111111111113');


INSERT INTO "Reservations" (
    "Id", "StartDate", "EndDate", "ServiceDateTime",
    "RoomId", "UserId", "HotelServiceId",
    "CreatedById", "DateCreated", "LastModifiedById", "DateLastModified"
) VALUES
      ('77777777-7777-7777-7777-777777777770', '2026-04-01 14:00:00', '2026-04-05 11:00:00', '2026-04-02 10:00:00',
       '22222222-2222-2222-2222-222222222222', '86EA686E-E4DA-41F0-9D28-9319FE0430DD', '44444444-4444-4444-4444-444444444444',
       '86EA686E-E4DA-41F0-9D28-9319FE0430DD', '2026-03-01 10:00:00', NULL, NULL),

      ('77777777-7777-7777-7777-777777777771', '2026-04-10 15:00:00', '2026-04-12 11:00:00', '2026-04-11 09:00:00',
       '22222222-2222-2222-2222-222222222224', '58073CE9-7EC2-46EB-9AFE-FB273A005592', '44444444-4444-4444-4444-444444444445',
       '58073CE9-7EC2-46EB-9AFE-FB273A005592', '2026-03-05 09:00:00', NULL, NULL),

      ('77777777-7777-7777-7777-777777777772', '2026-05-20 15:00:00', '2026-05-27 11:00:00', NULL,
       '22222222-2222-2222-2222-222222222221', 'C0AE787E-2A34-4081-9F80-5DCB436A8979', NULL,
       'C0AE787E-2A34-4081-9F80-5DCB436A8979', '2026-03-10 08:00:00', '676AE196-2094-45E7-BC6C-F2483B8AE7B8', NULL);

