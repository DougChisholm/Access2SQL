param(
    [string]$AccessFile = "C:\Users\dchisholm\modernise\input\app.accdb",
    [string]$OutputDir = "C:\Users\dchisholm\modernise\output\reports"
)

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
