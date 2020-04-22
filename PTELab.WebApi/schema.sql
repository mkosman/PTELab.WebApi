
    PRAGMA foreign_keys = OFF

    drop table if exists Company

    drop table if exists Employees

    PRAGMA foreign_keys = ON

    create table Company (
        Id  integer primary key autoincrement,
       Name TEXT not null,
       EstablishmentYear INT
    )

    create table Employees (
        Id  integer primary key autoincrement,
       FirstName TEXT not null,
       LastName TEXT not null,
       DateOfBirth DATETIME,
       JobTitle TEXT not null,
       Company_id BIGINT,
       constraint FK_C37E2C35 foreign key (Company_id) references Company
    )
