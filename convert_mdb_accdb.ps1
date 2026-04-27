param(
    [string]$SourceMdb = "C:\Users\dchisholm\modernise\input\app.mdb",
    [string]$TargetAccdb = "C:\Users\dchisholm\modernise\input\app.accdb"
)

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
