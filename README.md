# Group Management Platform (ITPE3200 – Exam Project, Task 3)

This is Subapp 1 (ASP.NET Core MVC) of the exam group project. 

## Group members

- Miriam Wang (mijit7782)
- Johannes Jonssønn Skramstad (joskr6803)
- Kaya Rabino Baksaas (kabak8529)
- Truls Andreassen (trand7816)
- Martin Ngo (mango7778)

## Requirements

- .NET 10 SDK (tested with 10.0.203)
- Node.js: not used. This is a pure ASP.NET Core MVC app with no npm dependencies. Bootstrap and jQuery are included as static files in `wwwroot/lib`.
- Database: SQLite (`groups.db`), created with EF Core migrations.
  
## How to run

From the repository root, run:

```bash
cd itpe3200
dotnet restore
dotnet tool restore
dotnet ef database update
dotnet run
```

### Start over with a fresh database

If the database is from an older version of the app (or you just want the test data back),
delete `groups.db` and create it again:

```bash
dotnet ef database update
dotnet run
```

## Test data

Course sessions:

| Session | Join code |
|---|---|
| ITPE3200 | `ABC123` |
| DATA1700 | `XYZ789` |
| DAPE1400 | `DEF456` |

Groups and members:

| Group | Session | Max size | Members |
|---|---|---|---|
| Gruppe1 | ITPE3200 | 4 | Elev1, Ola Nordmann |
| Gruppe2 | ITPE3200 | 3 | Kari Nordmann, Per Hansen, Anne Larsen (full) |
| Gruppe3 | ITPE3200 | 5 | Jonas Berg |
| Gruppe4 | DATA1700 | 4 | Elev1, Kari Nordmann, Sofie Dahl |
| Gruppe5 | DATA1700 | 4 | Ola Nordmann, Emil Strand |
| Gruppe6 | DAPE1400 | 5 | Per Hansen, Ingrid Lie |

- Elev1, Ola, Kari and Per are in groups in two different sessions (many-to-many).
- Lukas Moen is not in any group.

## Features

- Create course sessions with a name, maximum group count and a random-assignment setting. A unique 6-character join code is generated automatically.
- View all course sessions and their join codes.
- Join a course session as a student using its join code.
- Full CRUD for groups:
  - **Create** a group for a course session with a name and maximum size
  - **Read** groups as a table or card grid, and open a details page with the member list
  - **Update** a group's name and maximum size
  - **Delete** a group after a confirmation page (the members are removed from the group, but the students are not deleted)
- Many-to-many relationship between students and groups through the `GroupMember` join table. A student can be in groups in several sessions.
- All database access goes through the repository (`DAL/CourseRepository.cs`) and is asynchronous.
- Server-side input validation on all forms, with error messages shown next to each field.
- Error handling and logging: all database operations are wrapped in try/catch and logged with ILogger/Serilog to the console and to `Logs/`. Unexpected errors show a friendly error page.

## Sources

The project is based on Baifan Zhou's ITPE3200 MyShop course demos (Oslomet 2026, https://github.com/Baifan-Zhou/ITPE3200-26H), from the first MVC demos up to and
including "Demo: Input validation" (29 Sept 2026). The code follows the structure of the demos.


