# Saf siyah-beyaz profil görsellerini üretir (GDI+, anti-alias kapalı, 2x çözünürlük).
# Kullanım: powershell -File tools/render.ps1
Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = 'Stop'
$out = Join-Path (Split-Path $PSScriptRoot) 'assets'
New-Item -ItemType Directory -Force $out | Out-Null
Get-ChildItem $out -Include *.png,*.svg -Recurse | Remove-Item -Force

$S = 2
$black = [System.Drawing.Color]::FromArgb(255,0,0,0)
$white = [System.Drawing.Color]::FromArgb(255,255,255,255)
$HEAVY = 'Arial Black'
$CAPS  = 'Bahnschrift'
$FBODY = 'Segoe UI'

function New-Canvas($w, $h, $paper) {
  $bmp = [System.Drawing.Bitmap]::new([int]($w * $S), [int]($h * $S))
  $g = [System.Drawing.Graphics]::FromImage($bmp)
  $g.SmoothingMode = 'None'
  $g.PixelOffsetMode = 'Half'
  $g.TextRenderingHint = 'SingleBitPerPixelGridFit'
  $g.ScaleTransform($S, $S)
  $g.Clear($paper)
  return @{ Bmp = $bmp; G = $g }
}

$fmt = [System.Drawing.StringFormat]::GenericTypographic
$fmt.FormatFlags = $fmt.FormatFlags -bor [System.Drawing.StringFormatFlags]::MeasureTrailingSpaces

function Get-Advance($g, $text, $font, $track) {
  $w = 0.0
  foreach ($ch in $text.ToCharArray()) { $w += $g.MeasureString([string]$ch, $font, 10000, $fmt).Width + $track }
  return $w - $track
}
# harf harf çizer (letter-spacing); $align: left|right|center. y = satır kutusunun üstü
function Draw-Text($g, $text, $font, $brush, $x, $y, $track = 0, $align = 'left') {
  $w = Get-Advance $g $text $font $track
  if ($align -eq 'right') { $x = $x - $w } elseif ($align -eq 'center') { $x = $x - $w / 2 }
  if ($track -eq 0 -and $align -eq 'left') {
    $g.DrawString($text, $font, $brush, [single]$x, [single]$y, $fmt)
    return $w
  }
  foreach ($ch in $text.ToCharArray()) {
    $g.DrawString([string]$ch, $font, $brush, [single]$x, [single]$y, $fmt)
    $x += $g.MeasureString([string]$ch, $font, 10000, $fmt).Width + $track
  }
  return $w
}
function Fnt($name, $px, $bold = $false) {
  $style = if ($bold) { [System.Drawing.FontStyle]::Bold } else { [System.Drawing.FontStyle]::Regular }
  return [System.Drawing.Font]::new($name, [single]$px, $style, [System.Drawing.GraphicsUnit]::Pixel)
}
function Hatch($g, $x, $y, $w, $h, $ink, $gap = 10, $lw = 2) {
  $pen = New-Object System.Drawing.Pen $ink, $lw
  $state = $g.Save()
  $g.SetClip((New-Object System.Drawing.RectangleF $x, $y, $w, $h))
  for ($k = -$h; $k -lt $w + $h; $k += $gap) { $g.DrawLine($pen, [single]($x + $k), [single]($y + $h), [single]($x + $k + $h), [single]$y) }
  $g.Restore($state)
}
function Rect($g, $brush, $x, $y, $w, $h) { $g.FillRectangle($brush, [single]$x, [single]$y, [single]$w, [single]$h) }
function Frame($g, $ink, $w, $h, $t) {
  $b = New-Object System.Drawing.SolidBrush $ink
  Rect $g $b 0 0 $w $t; Rect $g $b 0 ($h - $t) $w $t; Rect $g $b 0 0 $t $h; Rect $g $b ($w - $t) 0 $t $h
}
function Save($c, $name) { $c.G.Dispose(); $c.Bmp.Save((Join-Path $out $name), [System.Drawing.Imaging.ImageFormat]::Png); $c.Bmp.Dispose() }

$themes = @{ light = @{ paper = $white; ink = $black }; dark = @{ paper = $black; ink = $white } }

# ---------- BANNER 1280x400 ----------
foreach ($name in $themes.Keys) {
  $paper = $themes[$name].paper; $ink = $themes[$name].ink
  $c = New-Canvas 1280 400 $paper; $g = $c.G
  $bi = New-Object System.Drawing.SolidBrush $ink; $bp = New-Object System.Drawing.SolidBrush $paper
  Frame $g $ink 1280 400 4
  $small = Fnt $CAPS 14 $true
  Draw-Text $g 'PORTFOLIO — 2026' $small $bi 40 30 3 | Out-Null
  Draw-Text $g 'AI SYSTEMS  /  AGENTS  /  TOOLING' $small $bi 640 30 3 'center' | Out-Null
  Draw-Text $g 'TÜRKİYE — OPEN SOURCE' $small $bi 1240 30 3 'right' | Out-Null
  Rect $g $bi 0 62 1280 3
  # wordmark: genişliğe tam oturt
  $txt = 'ArnoldGods'; $n = $txt.Length
  $probe = Fnt $HEAVY 100
  $adv100 = Get-Advance $g $txt $probe 0
  $size = [math]::Min(1204 / ($adv100 / 100 - ($n - 1) * 0.005), 190)
  $wm = Fnt $HEAVY $size
  $ffam = [System.Drawing.FontFamily]::new($HEAVY); $asc = $ffam.GetCellAscent([System.Drawing.FontStyle]::Regular) / $ffam.GetEmHeight([System.Drawing.FontStyle]::Regular) * $size
  $adv = Get-Advance $g $txt $wm (-0.005 * $size); Write-Host "wordmark size=$size width=$adv"
  Draw-Text $g $txt $wm $bi (642 - $adv / 2) (246 - $asc) (-0.005 * $size) | Out-Null
  Rect $g $bi 0 262 1280 3
  Rect $g $bi 0 265 884 135
  $slog = Fnt $HEAVY 38
  Draw-Text $g 'AI systems,' $slog $bp 40 282 -1 | Out-Null
  Draw-Text $g 'built in the open.' $slog $bp 40 328 -1 | Out-Null
  Hatch $g 884 265 396 135 $ink 12 2
  Rect $g $bi 881 265 3 135
  Rect $g $bp 930 300 300 64
  $pen = New-Object System.Drawing.Pen $ink, 3; $g.DrawRectangle($pen, 931.5, 301.5, 297, 61)
  Draw-Text $g 'BUILD. TEST. SHARE.' (Fnt $CAPS 18 $true) $bi 1080 321 3 'center' | Out-Null
  Save $c "banner-$name.png"
}

# ---------- PROJE KARTLARI 620x260 ----------
$cards = @(
  @{ id='agent-hafiza';  no='01'; tag='MEMORY';      d1='Oturumlar arasında bağlamı koruyan,';        d2='Markdown tabanlı yerel ikinci beyin.' },
  @{ id='claude-skills'; no='02'; tag='WORKFLOWS';   d1='Günlük işlerden çıkan, yeniden kullanılabilir'; d2='ajan becerileri: kod, review, araştırma.' },
  @{ id='model-arena';   no='03'; tag='EXPERIMENTS'; d1='Aynı prompt, farklı modeller. Orijinal';       d2='çıktılar ve maliyet açıkta.' },
  @{ id='terminal-kit';  no='04'; tag='TOOLS';       d1='Terminal ve çoklu-ajan iş akışları için';    d2='küçük, hızlı komut satırı araçları.' }
)
foreach ($name in $themes.Keys) {
  $paper = $themes[$name].paper; $ink = $themes[$name].ink
  foreach ($cd in $cards) {
    $c = New-Canvas 620 260 $paper; $g = $c.G
    $bi = New-Object System.Drawing.SolidBrush $ink; $bp = New-Object System.Drawing.SolidBrush $paper
    Frame $g $ink 620 260 4
    Draw-Text $g $cd.tag (Fnt $CAPS 14 $true) $bi 26 28 4 | Out-Null
    Rect $g $bi 26 52 40 4
    # içi boş büyük numara
    $gp = New-Object System.Drawing.Drawing2D.GraphicsPath
    $ff = New-Object System.Drawing.FontFamily $HEAVY
    $gp.AddString($cd.no, $ff, 0, [single]112, (New-Object System.Drawing.PointF 424, 6), $fmt)
    $pen = New-Object System.Drawing.Pen $ink, 2.5
    $g.DrawPath($pen, $gp)
    Draw-Text $g $cd.id (Fnt $HEAVY 40) $bi 24 80 -1.5 | Out-Null
    $bodyF = Fnt $FBODY 16
    Draw-Text $g $cd.d1 $bodyF $bi 26 148 0 | Out-Null
    Draw-Text $g $cd.d2 $bodyF $bi 26 172 0 | Out-Null
    Rect $g $bi 0 204 620 56
    Draw-Text $g 'DEPOYU AÇ' (Fnt $CAPS 16 $true) $bp 26 221 4 | Out-Null
    $ap = New-Object System.Drawing.Pen $paper, 3
    $g.DrawLine($ap, 404, 231, 444, 231); $g.DrawLine($ap, 444, 231, 430, 217); $g.DrawLine($ap, 444, 231, 430, 245)
    Hatch $g 470 204 150 56 $paper 9 2
    Rect $g $bp 468 204 3 56
    Save $c "card-$($cd.id)-$name.png"
  }
}

# ---------- ARAÇ ÇANTASI ŞERİDİ ----------
$stack = 'CLAUDE CODE','NODE.JS','TYPESCRIPT','PYTHON','GIT','OBSIDIAN'
foreach ($name in $themes.Keys) {
  $paper = $themes[$name].paper; $ink = $themes[$name].ink
  $f = Fnt $CAPS 15 $true
  $tmp = New-Canvas 10 10 $paper
  $widths = $stack | ForEach-Object { [math]::Ceiling((Get-Advance $tmp.G $_ $f 2) + 44) }
  $tmp.G.Dispose(); $tmp.Bmp.Dispose()
  $total = ($widths | Measure-Object -Sum).Sum - 3 * ($stack.Count - 1) + 6
  $c = New-Canvas $total 56 $paper; $g = $c.G
  $x = 0
  for ($i = 0; $i -lt $stack.Count; $i++) {
    $inv = ($i % 2 -eq 1)
    $fill = if ($inv) { $ink } else { $paper }; $txtc = if ($inv) { $paper } else { $ink }
    $bf = New-Object System.Drawing.SolidBrush $fill; $bt = New-Object System.Drawing.SolidBrush $txtc
    Rect $g $bf ($x + 1) 1 ($widths[$i]) 54
    $pen = New-Object System.Drawing.Pen $ink, 3
    $g.DrawRectangle($pen, [single]($x + 2.5), 2.5, [single]($widths[$i] - 3), 51)
    Draw-Text $g $stack[$i] $f $bt ($x + 3 + $widths[$i] / 2) 18 2 'center' | Out-Null
    $x += $widths[$i] - 3
  }
  Save $c "stack-$name.png"
}
Write-Host 'ok'
