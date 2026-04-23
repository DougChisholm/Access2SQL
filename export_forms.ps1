$accessFile = "C:\Users\dchisholm\modernise\input\app.accdb"
$outputDir  = "C:\Users\dchisholm\modernise\output\forms"

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
