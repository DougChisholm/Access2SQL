# School Tests — ASP.NET Core Web Application

Migrated from the **SchoolTests Microsoft Access database** to a modern ASP.NET Core 10 web application.

## 🚀 Quick Start

```bash
cd SchoolTests/src/WebApp
dotnet run
```

The app auto-creates the SQLite database (`schooltests.db`) and seeds sample data on first run.  
Navigate to **http://localhost:5265** (or whatever port is shown in the console).

## 🔐 Default Login

| Field    | Value                       |
|----------|-----------------------------|
| Email    | `admin@schooltests.local`   |
| Password | `Admin123!`                 |

> Authentication is **optional** — core features work without logging in.

## 📋 Application Workflow

This mirrors the original Access application:

1. Click **Enter Results** in the navigation (or go to `/ChooseTest`)
2. **Select a test** from the list → the "Enter Results" button activates
3. Click **Enter Results →** to open the mark-entry form
4. Enter marks (0–100) for each student, leave blank if not assessed
5. Click **💾 Save All Marks**

## 🏗️ Project Structure

```
SchoolTests/
├── src/
│   └── WebApp/                    # ASP.NET Core 10 Razor Pages
│       ├── Domain/                # EF Core entity models
│       │   ├── Student.cs
│       │   ├── Test.cs
│       │   ├── StudentResult.cs
│       │   └── AppInfo.cs         # Maps to Access README table
│       ├── Infrastructure/
│       │   ├── SchoolTestsDbContext.cs
│       │   └── DbInitializer.cs   # Seeds sample data
│       ├── Pages/
│       │   ├── Index.cshtml       # Dashboard
│       │   ├── ChooseTest.cshtml  # ← Access: ChooseTest form
│       │   ├── EnterResults.cshtml # ← Access: StudentResult form
│       │   ├── Students/          # CRUD for students
│       │   ├── Tests/             # CRUD for tests
│       │   └── Account/           # Login / Logout
│       ├── wwwroot/css/site.css   # GitHub-inspired design
│       └── Program.cs
├── LegacyMapping/
│   └── access-forms-mapping.json  # Documents Access → Web translation
└── SchoolTests.sln
```

## 🗂️ Access → Web Mapping

| Access Object           | Web Equivalent                          |
|-------------------------|-----------------------------------------|
| `ChooseTest` form       | `/ChooseTest` Razor Page                |
| `lstTest` list box      | Radio-button list with JavaScript       |
| `cmdEnterResults` button | Submit button (disabled until selection)|
| `StudentResult` form    | `/EnterResults` Razor Page              |
| `StudentInTest` query   | EF Core LINQ left join                  |
| `FilterResult` query    | EF Core `.Where(x => x.TestId == id)`   |
| `Form_Current` event    | JavaScript auto-focus on first input    |
| `Mark_BeforeUpdate`     | `[BindProperty] TestId` in POST handler |
| `DoCmd.OpenForm`        | `RedirectToPage` / form GET action      |

## 🛠️ Tech Stack

- **ASP.NET Core 10** — Razor Pages
- **Entity Framework Core 10** — ORM
- **SQLite** — Database (file: `schooltests.db`)
- **ASP.NET Core Identity** — Authentication
- **jQuery Validation** — Client-side validation
- **Custom CSS** — GitHub-inspired design system

## 📊 Database Schema

```sql
Student       (StudentId PK, StudentName)
Test          (TestId PK, TestDescription)
StudentResult (ResultId PK, StudentId FK, TestId FK, Mark)
README        (Item PK, Comment)      -- AppInfo/metadata
```

## 🧪 Build & Run

```bash
# Build
cd SchoolTests
dotnet build

# Run
cd src/WebApp
dotnet run
```
