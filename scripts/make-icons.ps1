# Generates the PWA icons into src/PreBot.Web/wwwroot/icons.
# Run: powershell -ExecutionPolicy Bypass -File .\scripts\make-icons.ps1
Add-Type -AssemblyName System.Drawing

$out = Join-Path $PSScriptRoot "..\src\PreBot.Web\wwwroot\icons"
New-Item -ItemType Directory -Force $out | Out-Null

function New-Icon([int]$size, [string]$file, [bool]$maskable, [bool]$badge) {
    $bmp = New-Object System.Drawing.Bitmap $size, $size
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = 'AntiAlias'
    $g.TextRenderingHint = 'AntiAliasGridFit'
    $g.Clear([System.Drawing.Color]::Transparent)

    if (-not $badge) {
        $bg = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 11, 18, 32))
        if ($maskable) {
            # Maskable icons must fill the whole square; the OS applies its own shape.
            $g.FillRectangle($bg, 0, 0, $size, $size)
        } else {
            $r = [int]($size * 0.22)
            $path = New-Object System.Drawing.Drawing2D.GraphicsPath
            $path.AddArc(0, 0, $r * 2, $r * 2, 180, 90)
            $path.AddArc($size - $r * 2, 0, $r * 2, $r * 2, 270, 90)
            $path.AddArc($size - $r * 2, $size - $r * 2, $r * 2, $r * 2, 0, 90)
            $path.AddArc(0, $size - $r * 2, $r * 2, $r * 2, 90, 90)
            $path.CloseFigure()
            $g.FillPath($bg, $path)
        }
        $ring = New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb(255, 200, 170, 110)), ([float]($size * 0.035))
        $m = $size * 0.2
        $g.DrawEllipse($ring, [float]$m, [float]$m, [float]($size - 2 * $m), [float]($size - 2 * $m))
        $color = [System.Drawing.Color]::FromArgb(255, 240, 212, 138)
    } else {
        # Android status-bar badge: white on transparent.
        $color = [System.Drawing.Color]::White
    }

    $fontSize = if ($badge) { $size * 0.8 } else { $size * 0.42 }
    $font = New-Object System.Drawing.Font "Segoe UI", ([float]$fontSize), ([System.Drawing.FontStyle]::Bold), ([System.Drawing.GraphicsUnit]::Pixel)
    $fmt = New-Object System.Drawing.StringFormat
    $fmt.Alignment = 'Center'
    $fmt.LineAlignment = 'Center'
    $rect = New-Object System.Drawing.RectangleF 0, ([float]($size * 0.03)), $size, $size
    $g.DrawString("5", $font, (New-Object System.Drawing.SolidBrush $color), $rect, $fmt)

    $bmp.Save((Join-Path $out $file), [System.Drawing.Imaging.ImageFormat]::Png)
    $g.Dispose(); $bmp.Dispose()
}

New-Icon 192 "icon-192.png" $false $false
New-Icon 512 "icon-512.png" $false $false
New-Icon 512 "icon-maskable-512.png" $true $false
New-Icon 180 "apple-touch-icon.png" $true $false
New-Icon 72 "badge-72.png" $false $true
New-Icon 32 "favicon-32.png" $false $false
Write-Host "Icons written to $out"
