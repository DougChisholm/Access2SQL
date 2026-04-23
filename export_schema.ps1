$accessFile = "C:\Users\dchisholm\input\app.accdb"
$outputFile = "C:\Users\dchisholm\output\schema.sql"

$access = New-Object -ComObject Access.Application
$access.OpenCurrentDatabase($accessFile)

$catalog = $access.CurrentDb.TableDefs

$output = ""

foreach ($table in $catalog) {
    if ($table.Attributes -eq 0) {
        $output += "TABLE: " + $table.Name + "`n"

        foreach ($field in $table.Fields) {
            $output += "  " + $field.Name + " : " + $field.Type + "`n"
        }

        $output += "`n"
    }
}

Set-Content -Path $outputFile -Value $output

$access.Quit()