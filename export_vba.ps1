$accessFile = "C:\Users\dchisholm\input\app.accdb"
$outputDir = "C:\Users\dchisholm\output\vba"

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