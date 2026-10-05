# Group Management Platform (ITPE3200 – Mandatory Assignment, Task 3)

## Group members

- Miriam Wang (mijit7782)
- Johannes Jonssønn Skramstad (joskr6803)
- Kaya Rabino Baksaas (kabak8529)
- Truls Andreassen (trand7816)
- Martin Ngo (mango7778)

## Requirements

- .NET 10 SDK
- Node.js: not used. This is a pure ASP.NET Core MVC app with no npm dependencies. Bootstrap and jQuery are included as static files in `wwwroot/lib`.
  
## How to run

From the repository root, run:

```bash
cd itpe3200
dotnet restore
dotnet run
```

## Test data

| Type | Value |
|---|---|
| Course session | ITPE3200 (join code `ABC123`) |
| Group | Gruppe1 (max size 4) |
| Student | Elev1 (member of Gruppe1) |

## Features

- Create course sessions with a name, maximum group count and a random-assignment setting. A unique 6-character join code is generated automatically.
- View all course sessions and their join codes.
- Join a course session as a student using its join code.
- Full CRUD for groups:
  - **Create** a group for a course session with a name and maximum size
  - **Read** groups as a table or card grid, and open a details page with the member list
  - **Update** a group's name and maximum size
  - **Delete** a group after a confirmation page (members are kept, but left without a group)
- Server-side input validation on all forms, with error messages shown next to each field.
- Error handling and logging: all database operations are wrapped in try/catch and logged with ILogger/Serilog to the console and to `Logs/`. Unexpected errors show a friendly error page.

## Sources

The project is based on Baifan Zhou's ITPE3200 MyShop course demos (Oslomet 2026, https://github.com/Baifan-Zhou/ITPE3200-26H), from the first MVC demos up to and
including "Demo: Input validation" (29 Sept 2026). The code follows the structure of the demos.


