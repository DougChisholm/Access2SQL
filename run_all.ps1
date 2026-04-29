$RepoRoot = $PSScriptRoot
$InputDir = Join-Path $RepoRoot "input"
$OutputDir = Join-Path $RepoRoot "output"

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

# Step 1: If only an .mdb file exists, convert it to .accdb first
$accdbFiles = Get-ChildItem -Path $InputDir -Filter "*.accdb" -ErrorAction SilentlyContinue
$mdbFiles   = Get-ChildItem -Path $InputDir -Filter "*.mdb"   -ErrorAction SilentlyContinue

if ($accdbFiles.Count -eq 0 -and $mdbFiles.Count -gt 0) {
    Write-Host "No .accdb found. Converting .mdb to .accdb first..."
    & "$RepoRoot\convert_mdb_accdb.ps1" -SourceMdb $mdbFiles[0].FullName
    if ($LASTEXITCODE -ne 0) {
        Write-Error "MDB conversion failed. Aborting."
        exit 1
    }
    Write-Host "Conversion complete."
} elseif ($accdbFiles.Count -eq 0) {
    Write-Error "No .accdb or .mdb file found in $InputDir. Please place an Access database file there."
    exit 1
}

# Step 2: Run all export scripts
Write-Host "Exporting schema..."
powershell -ExecutionPolicy Bypass -File "$RepoRoot\export_schema.ps1"

Write-Host "Exporting forms..."
powershell -ExecutionPolicy Bypass -File "$RepoRoot\export_forms.ps1"

Write-Host "Exporting reports..."
powershell -ExecutionPolicy Bypass -File "$RepoRoot\export_reports.ps1"

Write-Host "Exporting VBA modules..."
powershell -ExecutionPolicy Bypass -File "$RepoRoot\export_vba.ps1"

Write-Host "Exporting queries..."
powershell -ExecutionPolicy Bypass -File "$RepoRoot\export_queries.ps1"

Write-Host ""
Write-Host "All exports complete. Artifacts written to: $OutputDir"

