$accessFile = "C:\Users\dchisholm\modernise\input\app.accdb"
$outputDir = "C:\Users\dchisholm\modernise\output\vba"

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
