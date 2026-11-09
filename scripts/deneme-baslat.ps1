# Arkadas denemesini TEK KOMUTLA baslatir: siteyi ve tuneli arka planda acar, internet adresini bulur
# ve arkadaslarina gonderilecek hazir mesaji yazar (ayrica davet-mesaji.txt dosyasina kaydeder).
#   powershell -ExecutionPolicy Bypass -File .\scripts\deneme-baslat.ps1
# Durdurmak icin:
#   powershell -ExecutionPolicy Bypass -File .\scripts\deneme-durdur.ps1
#
# -Tunel tailscale : Cloudflare yerine Tailscale Funnel kullanir. Adres SABIT kalir (ornek
#                    https://bilgisayar.tailXXXX.ts.net), bilgisayar yeniden baslasa da degismez; bildirimler bozulmaz.
#                    Once Tailscale kurulup hesapla giris yapilmis olmali.
#
# -DavetKodu : Google girisi ayarli olsa da "isim + davet kodu" girisini de acik tutar.
param(
    [ValidateSet("cloudflare", "tailscale")]
    [string]$Tunel = "cloudflare",
    [switch]$DavetKodu
)

$ErrorActionPreference = "Stop"
$data = Join-Path $env:LOCALAPPDATA "PreBot-deneme"
New-Item -ItemType Directory -Force $data | Out-Null

# Already running? Stop the old ones first, so there is exactly one site and one tunnel.
& "$PSScriptRoot\deneme-durdur.ps1" -Sessiz

Write-Host "Site baslatiliyor (ilk seferde derleme 1-2 dakika surebilir)..." -ForegroundColor Cyan
$siteLog = Join-Path $data "site.log"
Start-Process powershell -WindowStyle Minimized -ArgumentList @(
    "-ExecutionPolicy", "Bypass", "-Command",
    "& '$PSScriptRoot\deneme-sitesi.ps1' $(if ($DavetKodu) { '-DavetKodu' }) *> '$siteLog'")

$healthy = $false
for ($i = 0; $i -lt 180; $i++) {
    try {
        if ((Invoke-WebRequest -Uri "http://localhost:5290/health" -UseBasicParsing -TimeoutSec 2).Content -eq "Healthy") { $healthy = $true; break }
    } catch { }
    Start-Sleep -Seconds 1
}
if (-not $healthy) { Write-Host "Site acilamadi. Ayrintilar: $siteLog" -ForegroundColor Red; exit 1 }

if ($Tunel -eq "tailscale") {
    $ts = (Get-Command tailscale -ErrorAction SilentlyContinue).Source
    if (-not $ts) { $ts = Join-Path $env:ProgramFiles "Tailscale\tailscale.exe" }
    if (-not (Test-Path $ts)) { Write-Host "Tailscale kurulu degil: https://tailscale.com/download/windows" -ForegroundColor Red; exit 1 }

    # --bg: the funnel stays configured across restarts; only the site has to be started again.
    & $ts funnel --bg 5290
    $match = & $ts funnel status | Select-String -Pattern "https://[a-z0-9.-]+\.ts\.net" | Select-Object -First 1
    $url = if ($match) { $match.Matches[0].Value } else { $null }
    if (-not $url) { Write-Host "Funnel adresi alinamadi. 'tailscale funnel status' ciktisina bak." -ForegroundColor Red; exit 1 }
} else {
# The tunnel script downloads cloudflared on first use; run it once in the foreground if it is missing.
$exe = Join-Path $data "cloudflared.exe"
if (-not (Test-Path $exe)) {
    Write-Host "cloudflared ilk kez indirilecek; once deneme-tuneli.ps1'i bir kez calistir." -ForegroundColor Yellow
    exit 1
}

Write-Host "Tunel aciliyor..." -ForegroundColor Cyan
$tunnelLog = Join-Path $data "tunel.log"
Remove-Item $tunnelLog -ErrorAction SilentlyContinue
Start-Process $exe -WindowStyle Minimized -ArgumentList @("tunnel", "--no-autoupdate", "--url", "http://localhost:5290", "--logfile", $tunnelLog)

$url = $null
for ($i = 0; $i -lt 60 -and -not $url; $i++) {
    Start-Sleep -Seconds 1
    if (Test-Path $tunnelLog) {
        $m = Select-String -Path $tunnelLog -Pattern "https://[a-z0-9-]+\.trycloudflare\.com" | Select-Object -First 1
        if ($m) { $url = $m.Matches[0].Value }
    }
}
if (-not $url) { Write-Host "Tunel adresi alinamadi. Ayrintilar: $tunnelLog" -ForegroundColor Red; exit 1 }
}

$login = (Get-Content (Join-Path $data "giris.txt") -Raw -ErrorAction SilentlyContinue)
$code = (Get-Content (Join-Path $data "davet-kodu.txt") -Raw).Trim()
$signIn = if ("$login".Trim() -eq "google") {
@"
2) "Google ile giris"e bas (kutucugu isaretle). Google'dan yalnizca adini aliyoruz, e-postani almiyoruz.
"@
} else {
@"
2) Davet kodu: $code  - adini ve kendine bir sifre yaz, kutucugu isaretle, gir
   (sifreyi ilk giriste sen belirlersin; baska yerde kullandigin sifre olmasin)
"@
}
$message = @"
PreBot denemesi: Flex'te 4 kisi kalinca 5. kisiyi bulmak icin yaptigim site.

1) Telefondan ac: $url
   (iPhone'da Safari -> Paylas -> Ana Ekrana Ekle, sonra ana ekrandan ac; yoksa bildirim gelmez)
$signIn
3) Riot ID'ni kaydet ve "Bildirimleri ac"a bas
4) Havuz sayfasinda "Aksamlari bana haber ver"e bas (bir kez yeter);
   o an oynayabiliyorsan "Su an musaitim" de, once sana sorulsun.
   Mac ortasinda bildirim gelirse "Mactayim, sonra"ya bas.

4 kisi kaldigimizda ilan acacagiz, bildirim gelince "Katil"a bas. Ilk basan alir.
"@
$messageFile = Join-Path $data "davet-mesaji.txt"
Set-Content -Path $messageFile -Value $message -Encoding utf8

Write-Host ""
Write-Host "Hazir! Adres: $url" -ForegroundColor Green
if ("$login".Trim() -eq "google") { Write-Host "Giris: Google ile" -ForegroundColor Green } else { Write-Host "Davet kodu: $code" -ForegroundColor Green }
Write-Host "Arkadaslarina gonderilecek mesaj ($messageFile):" -ForegroundColor Green
Write-Host ""
Write-Host $message
Write-Host ""
Write-Host "Site ve tunel arka planda (kucultulmus pencerelerde) calisiyor. Bilgisayar acik kalmali." -ForegroundColor DarkGray
