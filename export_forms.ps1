$accessFile = "C:\Users\dchisholm\input\app.accdb"
$outputDir = "C:\Users\dchisholm\output\forms"

New-Item -ItemType Directory -Force -Path $outputDir | Out-Null

$access = New-Object -ComObject Access.Application
$access.OpenCurrentDatabase($accessFile)

$db = $access.CurrentDb
$forms = $db.Containers("Forms")

foreach ($doc in $forms.Documents) {
    $name = $doc.Name
    $path = Join-Path $outputDir ($name + ".txt")

    $access.SaveAsText(3, $name, $path)  # 3 = acForm
}

$access.Quit()