# Access2SQL

Export a Microsoft Access database (`.accdb` or `.mdb`) to SQL/text artifacts
using a single PowerShell script powered by Access COM automation.

## Quick start

1. Copy (or clone) this repository to any folder.
2. Place your `.accdb` **or** `.mdb` file in the **same folder** as the script.
3. Run:

```powershell
.\access2sql.ps1
```

That's it. All output is written into sub-folders next to the script.

## What gets exported

| Sub-folder      | Contents                                  |
|-----------------|-------------------------------------------|
| `schema\`       | `schema.sql` – table names and field list |
| `forms\`        | One `.txt` per form (Access SaveAsText)   |
| `reports\`      | One `.txt` per report                     |
| `vba\`          | One `.bas` per VBA module/class           |
| `queries-sql\`  | One `.sql` per query                      |

## MDB → ACCDB conversion

If only an `.mdb` file is present the script automatically converts it to
`.accdb` (via the Access COM `NewCurrentDatabase` / `TransferDatabase` API)
before running the exports. The converted `.accdb` is saved in the same
folder as the original `.mdb`.

## Parameters

| Parameter | Description |
|-----------|-------------|
| `-Source` | Path to a specific `.accdb` or `.mdb` file. Optional. When omitted the script searches its own directory and picks the first file alphabetically. |

### Examples

```powershell
# Auto-discover database in script folder
.\access2sql.ps1

# Point at an explicit file
.\access2sql.ps1 -Source "C:\Databases\MyApp.accdb"

# Explicit MDB (will be converted on the fly)
.\access2sql.ps1 -Source "C:\Databases\LegacyApp.mdb"
```

## Prerequisites

- Windows with Microsoft Access installed (32-bit or 64-bit).
- PowerShell 5.1 or later.
- The Access COM server (`Access.Application`) and DAO engine
  (`DAO.DBEngine.120`) must be registered (they are by default when Access is
  installed).

## Folder layout

```
Access2SQL\
├── access2sql.ps1          ← run this
├── MyDatabase.accdb        ← place your database here
├── schema\
│   └── schema.sql
├── forms\
│   ├── FormA.txt
│   └── FormB.txt
├── reports\
│   └── ReportA.txt
├── vba\
│   └── Module1.bas
└── queries-sql\
    └── MyQuery.sql
```

## Deprecated scripts

The following scripts from the original multi-script workflow are kept for
reference but are **no longer needed**:

| Script                  | Replacement                            |
|-------------------------|----------------------------------------|
| `run_all.ps1`           | `access2sql.ps1` (drop-in replacement) |
| `convert_mdb_accdb.ps1` | Built into `access2sql.ps1`            |
| `export_forms.ps1`      | Built into `access2sql.ps1`            |
| `export_schema.ps1`     | Built into `access2sql.ps1`            |
| `export_reports.ps1`    | Built into `access2sql.ps1`            |
| `export_vba.ps1`        | Built into `access2sql.ps1`            |
| `export_queries.ps1`    | Built into `access2sql.ps1`            |

## License

See repository for license details.
