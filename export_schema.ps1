$accessFile = "C:\Users\dchisholm\modernise\input\app.accdb"
$outputFile = "C:\Users\dchisholm\modernise\output\schema.sql"

# Start Access (only for COM context)
$access = New-Object -ComObject Access.Application
$access.OpenCurrentDatabase($accessFile)

Start-Sleep -Seconds 2

# 🔥 REAL DAO ENGINE (this is the fix)
$dao = New-Object -ComObject DAO.DBEngine.120
$db = $dao.OpenDatabase($accessFile)

$output = ""

foreach ($table in $db.TableDefs) {

    # skip system tables
    if ($table.Name -like "MSys*") { continue }

    $output += "TABLE: $($table.Name)`n"

    foreach ($field in $table.Fields) {
        $output += "  $($field.Name)`n"
    }

    $output += "`n"
}

# Force write
[System.IO.File]::WriteAllText($outputFile, $output)

$access.Quit()
