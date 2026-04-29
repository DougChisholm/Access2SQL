$RepoRoot = $PSScriptRoot
$InputDir = Join-Path $RepoRoot "input"
$OutputDir = Join-Path (Join-Path $RepoRoot "output") "forms"

$accdbFiles = Get-ChildItem -Path $InputDir -Filter "*.accdb" -ErrorAction SilentlyContinue
if ($accdbFiles.Count -eq 0) {
    Write-Error "No .accdb file found in $InputDir. Run convert_mdb_accdb.ps1 first if you have an .mdb file."
    exit 1
}
$accessFile = $accdbFiles[0].FullName
Write-Host "Using Access file: $accessFile"

$outputDir  = $OutputDir

New-Item -ItemType Directory -Force -Path $outputDir | Out-Null

$access = New-Object -ComObject Access.Application
$access.Visible = $false
$access.OpenCurrentDatabase($accessFile)

Start-Sleep -Seconds 3

foreach ($obj in $access.CurrentProject.AllForms) {

    $name = $obj.Name.Trim()
    $path = Join-Path $outputDir ($name + ".txt")

    try {
        # 🔥 test export capability BEFORE writing
     	$access.SaveAsText(2, $name, $path)
        Write-Host "Exported: $name"
    }
    catch {
        Write-Host "Skipped: $name (not real exportable form definition)"
    }
}

try { $access.Quit() } catch {}

[System.Runtime.InteropServices.Marshal]::ReleaseComObject($access) | Out-Null
[GC]::Collect()
[GC]::WaitForPendingFinalizers()

Write-Host "Done."
