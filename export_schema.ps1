$RepoRoot = $PSScriptRoot
$InputDir = Join-Path $RepoRoot "input"
$OutputDir = Join-Path $RepoRoot "output"
New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

$accdbFiles = Get-ChildItem -Path $InputDir -Filter "*.accdb" -ErrorAction SilentlyContinue
if ($accdbFiles.Count -eq 0) {
    Write-Error "No .accdb file found in $InputDir. Run convert_mdb_accdb.ps1 first if you have an .mdb file."
    exit 1
}
$accessFile = $accdbFiles[0].FullName
Write-Host "Using Access file: $accessFile"

$outputFile = Join-Path $OutputDir "schema.sql"

# Start Access (only for COM context)
$access = New-Object -ComObject Access.Application
$access.OpenCurrentDatabase($accessFile)

Start-Sleep -Seconds 2

# 🔥 REAL DAO ENGINE (this is the fix)
$dao = New-Object -ComObject DAO.DBEngine.120
$db = $dao.OpenDatabase($accessFile)

$output = ""

foreach ($table in $db.TableDefs) {

    # skip system tables
    if ($table.Name -like "MSys*") { continue }

    $output += "TABLE: $($table.Name)`n"

    foreach ($field in $table.Fields) {
        $output += "  $($field.Name)`n"
    }

    $output += "`n"
}

# Force write
[System.IO.File]::WriteAllText($outputFile, $output)

$access.Quit()
