param(
    [string]$SourceMdb,
    [string]$TargetAccdb
)

# Default: find first .mdb file in the repo input folder
$RepoRoot = $PSScriptRoot
$InputDir = Join-Path $RepoRoot "input"

if (-not $SourceMdb) {
    $mdbFiles = Get-ChildItem -Path $InputDir -Filter "*.mdb" -ErrorAction SilentlyContinue
    if ($mdbFiles.Count -eq 0) {
        Write-Error "No .mdb file found in $InputDir"
        exit 1
    }
    $SourceMdb = $mdbFiles[0].FullName
    Write-Host "Found MDB: $SourceMdb"
}

if (-not $TargetAccdb) {
    $baseName = [System.IO.Path]::GetFileNameWithoutExtension($SourceMdb)
    $TargetAccdb = Join-Path $InputDir ($baseName + ".accdb")
}

# Create Access COM object
$access = New-Object -ComObject Access.Application
$access.Visible = $false

try {
    Write-Host "Creating new ACCDB..."

    # Create blank ACCDB
    $access.NewCurrentDatabase($TargetAccdb)

    Write-Host "Importing MDB objects..."

    # Import EVERYTHING from MDB
    # acImport = 0, acTable = 0, acQuery = 5, etc. handled by Access import wizard internally
    $access.DoCmd.TransferDatabase(
        0,                      # acImport
        "Microsoft Access",
        $SourceMdb,
        0,                      # acTable (ignored when importing all objects via UI-style import)
        "",
        ""
    )

    Write-Host "Conversion complete: $TargetAccdb"
}
catch {
    Write-Error $_.Exception.Message
}
finally {
    try { $access.Quit() } catch {}
    [System.Runtime.InteropServices.Marshal]::ReleaseComObject($access) | Out-Null
    [GC]::Collect()
    [GC]::WaitForPendingFinalizers()
}
