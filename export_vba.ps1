# DEPRECATED: Use access2sql.ps1 instead, which exports VBA modules (and all
# other objects) without requiring "input" or "output" folders.

$RepoRoot = $PSScriptRoot
$InputDir = Join-Path $RepoRoot "input"
$OutputDir = Join-Path (Join-Path $RepoRoot "output") "vba"

$accdbFiles = Get-ChildItem -Path $InputDir -Filter "*.accdb" -ErrorAction SilentlyContinue
if ($accdbFiles.Count -eq 0) {
    Write-Error "No .accdb file found in $InputDir. Run convert_mdb_accdb.ps1 first if you have an .mdb file."
    exit 1
}
$accessFile = $accdbFiles[0].FullName
Write-Host "Using Access file: $accessFile"

$outputDir = $OutputDir

New-Item -ItemType Directory -Force -Path $outputDir | Out-Null

$access = New-Object -ComObject Access.Application
$access.OpenCurrentDatabase($accessFile)

$vbproj = $access.VBE.VBProjects.Item(1)

foreach ($component in $vbproj.VBComponents) {
    $name = $component.Name
    $file = Join-Path $outputDir ($name + ".bas")

    $component.Export($file)
}

$access.Quit()
