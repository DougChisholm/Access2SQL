<#
.SYNOPSIS
    Access2SQL – export a Microsoft Access database to SQL/text artifacts.

.DESCRIPTION
    Discovers an .accdb (or .mdb, which it converts on demand) in the directory
    where this script lives, then exports:
      - Table schema          → schema\schema.sql
      - Forms                 → forms\<name>.txt
      - Reports               → reports\<name>.txt
      - VBA modules           → vba\<name>.bas
      - Queries               → queries-sql\<name>.sql

    All output sub-folders are created automatically under the script directory.
    No "input" or "output" folders are required.

.PARAMETER Source
    Optional path to a specific .accdb or .mdb file.
    If omitted the script searches its own directory (alphabetical order, first match).

.EXAMPLE
    .\access2sql.ps1

.EXAMPLE
    .\access2sql.ps1 -Source "C:\dbs\MyApp.mdb"
#>

param(
    [string]$Source
)

$ScriptDir = $PSScriptRoot

# ── Helper: create/ensure an output sub-folder ────────────────────────────────
function New-OutputDir ([string]$Name) {
    $dir = Join-Path $ScriptDir $Name
    New-Item -ItemType Directory -Force -Path $dir | Out-Null
    return $dir
}

# ── Helper: release a COM object cleanly ──────────────────────────────────────
function Release-Com ($obj) {
    try { [System.Runtime.InteropServices.Marshal]::ReleaseComObject($obj) | Out-Null } catch {}
    [GC]::Collect()
    [GC]::WaitForPendingFinalizers()
}

# ── 1. Locate / prepare the .accdb ────────────────────────────────────────────
if ($Source) {
    if (-not (Test-Path $Source)) {
        Write-Error "Specified source file not found: $Source"
        exit 1
    }
    $accessFile = (Resolve-Path $Source).Path
} else {
    $accdbFiles = @(Get-ChildItem -Path $ScriptDir -Filter "*.accdb" -ErrorAction SilentlyContinue | Sort-Object Name)
    $mdbFiles   = @(Get-ChildItem -Path $ScriptDir -Filter "*.mdb"   -ErrorAction SilentlyContinue | Sort-Object Name)

    if ($accdbFiles.Count -gt 0) {
        $accessFile = $accdbFiles[0].FullName
        Write-Host "Found ACCDB: $accessFile"
    } elseif ($mdbFiles.Count -gt 0) {
        Write-Host "No .accdb found – converting MDB to ACCDB..."
        $sourceMdb  = $mdbFiles[0].FullName
        $baseName   = [System.IO.Path]::GetFileNameWithoutExtension($sourceMdb)
        $accessFile = Join-Path $ScriptDir ($baseName + ".accdb")

        $access = New-Object -ComObject Access.Application
        $access.Visible = $false
        try {
            Write-Host "Creating new ACCDB: $accessFile"
            $access.NewCurrentDatabase($accessFile)

            Write-Host "Importing objects from: $sourceMdb"
            $access.DoCmd.TransferDatabase(
                0,                   # acImport
                "Microsoft Access",
                $sourceMdb,
                0,                   # acTable (all objects)
                "",
                ""
            )
            Write-Host "Conversion complete: $accessFile"
        }
        catch {
            Write-Error "MDB conversion failed: $($_.Exception.Message)"
            try { $access.Quit() } catch {}
            Release-Com $access
            exit 1
        }
        finally {
            try { $access.Quit() } catch {}
            Release-Com $access
        }
    } else {
        Write-Error "No .accdb or .mdb file found in '$ScriptDir'. Place a database file there or use -Source."
        exit 1
    }
}

Write-Host "Using Access database: $accessFile"

# ── 2. Export schema ──────────────────────────────────────────────────────────
function Export-Schema ([string]$AccessFile) {
    Write-Host "`n--- Exporting schema ---"
    $outputDir  = New-OutputDir "schema"
    $outputFile = Join-Path $outputDir "schema.sql"

    $access = New-Object -ComObject Access.Application
    $access.Visible = $false
    try {
        $access.OpenCurrentDatabase($AccessFile)
        Start-Sleep -Seconds 2

        # Use DAO directly for reliable field enumeration
        $dao = New-Object -ComObject DAO.DBEngine.120
        $db  = $dao.OpenDatabase($AccessFile)

        $output = ""
        foreach ($table in $db.TableDefs) {
            if ($table.Name -like "MSys*") { continue }
            $output += "TABLE: $($table.Name)`n"
            foreach ($field in $table.Fields) {
                $output += "  $($field.Name)`n"
            }
            $output += "`n"
        }

        [System.IO.File]::WriteAllText($outputFile, $output)
        Write-Host "Schema written to: $outputFile"
    }
    finally {
        try { $access.Quit() } catch {}
        Release-Com $access
    }
}

# ── 3. Export forms ───────────────────────────────────────────────────────────
function Export-Forms ([string]$AccessFile) {
    Write-Host "`n--- Exporting forms ---"
    $outputDir = New-OutputDir "forms"

    $access = New-Object -ComObject Access.Application
    $access.Visible = $false
    try {
        $access.OpenCurrentDatabase($AccessFile)
        Start-Sleep -Seconds 3

        foreach ($obj in $access.CurrentProject.AllForms) {
            $name = $obj.Name.Trim()
            $path = Join-Path $outputDir ($name + ".txt")
            try {
                # 2 = acForm
                $access.SaveAsText(2, $name, $path)
                Write-Host "Exported form: $name"
            }
            catch {
                Write-Host "Skipped form: $name (not exportable)"
            }
        }
    }
    finally {
        try { $access.Quit() } catch {}
        Release-Com $access
    }
}

# ── 4. Export reports ─────────────────────────────────────────────────────────
function Export-Reports ([string]$AccessFile) {
    Write-Host "`n--- Exporting reports ---"
    $outputDir = New-OutputDir "reports"

    $access = New-Object -ComObject Access.Application
    $access.Visible = $false
    try {
        $access.OpenCurrentDatabase($AccessFile)

        foreach ($report in $access.CurrentProject.AllReports) {
            $name     = $report.Name
            $safeName = $name -replace '[^a-zA-Z0-9_-]', '_'
            $filePath = Join-Path $outputDir ($safeName + ".txt")
            try {
                # 3 = acReport
                $access.SaveAsText(3, $name, $filePath)
                Write-Host "Exported report: $name"
            }
            catch {
                Write-Warning "Failed to export report: $name"
            }
        }
    }
    finally {
        try { $access.CloseCurrentDatabase() } catch {}
        try { $access.Quit() } catch {}
        Release-Com $access
    }
}

# ── 5. Export VBA modules ─────────────────────────────────────────────────────
function Export-VBA ([string]$AccessFile) {
    Write-Host "`n--- Exporting VBA modules ---"
    $outputDir = New-OutputDir "vba"

    $access = New-Object -ComObject Access.Application
    $access.Visible = $false
    try {
        $access.OpenCurrentDatabase($AccessFile)

        $vbproj = $access.VBE.VBProjects.Item(1)
        foreach ($component in $vbproj.VBComponents) {
            $name = $component.Name
            $file = Join-Path $outputDir ($name + ".bas")
            $component.Export($file)
            Write-Host "Exported VBA module: $name"
        }
    }
    finally {
        try { $access.Quit() } catch {}
        Release-Com $access
    }
}

# ── 6. Export queries ─────────────────────────────────────────────────────────
function Export-Queries ([string]$AccessFile) {
    Write-Host "`n--- Exporting queries ---"
    $outputDir = New-OutputDir "queries-sql"

    $access = New-Object -ComObject Access.Application
    $access.Visible = $false
    try {
        $access.OpenCurrentDatabase($AccessFile)
        Start-Sleep -Seconds 3

        $db = $access.CurrentDb()
        foreach ($query in $db.QueryDefs) {
            $name = $query.Name.Trim()

            # Skip system/temp queries
            if ($name.StartsWith("~") -or $name.StartsWith("MSys")) { continue }

            # Skip queries with no SQL
            if ([string]::IsNullOrWhiteSpace($query.SQL)) { continue }

            $safeName = $name -replace '[^a-zA-Z0-9_-]', '_'
            $path     = Join-Path $outputDir ($safeName + ".sql")
            try {
                $query.SQL | Out-File -FilePath $path -Encoding UTF8
                Write-Host "Exported query: $name"
            }
            catch {
                Write-Host "Failed to export query: $name"
            }
        }
    }
    finally {
        try { $access.Quit() } catch {}
        Release-Com $access
    }
}

# ── Run all exports ───────────────────────────────────────────────────────────
Export-Schema  $accessFile
Export-Forms   $accessFile
Export-Reports $accessFile
Export-VBA     $accessFile
Export-Queries $accessFile

Write-Host "`nAll exports complete. Artifacts written to: $ScriptDir"
