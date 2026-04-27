$accessFile = "C:\Users\dchisholm\modernise\input\app.accdb"
$outputDir  = "C:\Users\dchisholm\modernise\output\queries-sql"

New-Item -ItemType Directory -Force -Path $outputDir | Out-Null

$access = New-Object -ComObject Access.Application
$access.Visible = $false
$access.OpenCurrentDatabase($accessFile)

Start-Sleep -Seconds 3

$db = $access.CurrentDb()

foreach ($query in $db.QueryDefs) {

    $name = $query.Name.Trim()

    # Skip system/temp queries
    if ($name.StartsWith("~") -or $name.StartsWith("MSys")) {
        continue
    }

    # Skip queries with no SQL
    if ([string]::IsNullOrWhiteSpace($query.SQL)) {
        continue
    }

    # Clean filename
    $safeName = $name -replace '[^a-zA-Z0-9_-]', '_'
    $path = Join-Path $outputDir ($safeName + ".sql")

    try {
        $query.SQL | Out-File -FilePath $path -Encoding UTF8
        Write-Host "Exported SQL: $name"
    }
    catch {
        Write-Host "Failed: $name"
    }
}

try { $access.Quit() } catch {}

[System.Runtime.InteropServices.Marshal]::ReleaseComObject($access) | Out-Null
[GC]::Collect()
[GC]::WaitForPendingFinalizers()

Write-Host "Done."
