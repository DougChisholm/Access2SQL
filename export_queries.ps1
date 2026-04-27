param(
    [string]$AccessFile = "C:\Users\dchisholm\modernise\input\app.accdb",
    [string]$OutputDir = "C:\Users\dchisholm\modernise\output\queries"
)

# Ensure output directory exists
New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

# Start Access COM
$access = New-Object -ComObject Access.Application
$access.Visible = $false

try {
    $access.OpenCurrentDatabase($AccessFile)

    Write-Host "Exporting queries from: $AccessFile"

    foreach ($query in $access.CurrentDb().QueryDefs) {

        $name = $query.Name

        # Skip system / temp queries
        if ($name.StartsWith("~") -or $name.StartsWith("MSys")) {
            continue
        }

        # Optional: sanitise filename
        $safeName = $name -replace '[^a-zA-Z0-9_-]', '_'
        $filePath = Join-Path $OutputDir ($safeName + ".txt")

        try {
            # 5 = acQuery
            $access.SaveAsText(5, $name, $filePath)
            Write-Host "Exported query: $name"
        }
        catch {
            Write-Warning "Failed to export query: $name"
        }
    }
}
finally {
    # Cleanup
    $access.CloseCurrentDatabase()
    $access.Quit()
    [System.Runtime.Interopservices.Marshal]::ReleaseComObject($access) | Out-Null
}

Write-Host "Query export complete."
