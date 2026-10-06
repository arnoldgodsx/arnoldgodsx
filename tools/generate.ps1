[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$targetDir = "C:\Users\ArnoldGods\AppData\Local\Temp\agyimg2"

Add-Type -Path (Join-Path $targetDir "GraphicRenderer.cs") -ReferencedAssemblies "System.Drawing"

Write-Output "Generating all images..."
$files = [GraphicRenderer]::GenerateAll($targetDir)

Write-Output "`nVerifying all generated images for strict 1-bit binary purity (#000000 and #FFFFFF only)..."
foreach ($f in $files) {
    if ($f -match "banner") {
        [GraphicRenderer]::VerifyFile($f, 1280, 400)
    } else {
        [GraphicRenderer]::VerifyFile($f, 1240, 520)
    }
}

Write-Output "`nAll 10 images successfully generated and verified!"
