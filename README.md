# Group Management Platform (ITPE3200 – Mandatory Assignment, Task 3)

## Group members

mijit7782
kabak8529
joskr6803
mango7778
trand7816

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

- Create course sessions with a name, maximum group count, and a random-assignment setting.
- View course sessions and their join codes.
- Join a course session as a student using its join code.
- Create groups for a course session, with a group name and maximum size.
- View groups as a table or card grid, including session, member count, and capacity.

## Sources

The project is based on Baifan Zhou's ITPE3200 MyShop course demos (Oslomet 2026, https://github.com/Baifan-Zhou/ITPE3200-26H), from the first MVC demos up to and
including "Demo: Input validation" (29 Sept 2026). The code follows the structure of the demos.


