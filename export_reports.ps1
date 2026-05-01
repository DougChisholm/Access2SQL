# DEPRECATED: Use access2sql.ps1 instead, which exports reports (and all other
# objects) without requiring "input" or "output" folders.

param(
    [string]$AccessFile,
    [string]$OutputDir
)

$RepoRoot = $PSScriptRoot
$InputDir = Join-Path $RepoRoot "input"

if (-not $AccessFile) {
    $accdbFiles = Get-ChildItem -Path $InputDir -Filter "*.accdb" -ErrorAction SilentlyContinue
    if ($accdbFiles.Count -eq 0) {
        Write-Error "No .accdb file found in $InputDir. Run convert_mdb_accdb.ps1 first if you have an .mdb file."
        exit 1
    }
    $AccessFile = $accdbFiles[0].FullName
    Write-Host "Using Access file: $AccessFile"
}

if (-not $OutputDir) {
    $OutputDir = Join-Path (Join-Path $RepoRoot "output") "reports"
}

# Ensure output directory exists
New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

# Start Access COM
$access = New-Object -ComObject Access.Application
$access.Visible = $false

try {
    $access.OpenCurrentDatabase($AccessFile)

    Write-Host "Exporting reports from: $AccessFile"

    foreach ($report in $access.CurrentProject.AllReports) {
        $name = $report.Name

        # Optional: sanitise filename
        $safeName = $name -replace '[^a-zA-Z0-9_-]', '_'

        $filePath = Join-Path $OutputDir ($safeName + ".txt")

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
    # Cleanup
    $access.CloseCurrentDatabase()
    $access.Quit()
    [System.Runtime.Interopservices.Marshal]::ReleaseComObject($access) | Out-Null
}

Write-Host "Report export complete."
