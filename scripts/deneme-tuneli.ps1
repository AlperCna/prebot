# Bu bilgisayarda calisan siteye (deneme-sitesi.ps1) internetten HTTPS ile erisim acar.
#   powershell -ExecutionPolicy Bypass -File .\scripts\deneme-tuneli.ps1
#
# Cloudflare "Quick Tunnel" kullanir: hesap ya da alan adi gerekmez, ucretsizdir. Ciktidaki
# https://....trycloudflare.com adresini arkadaslarina gonder. Adres her acilista degisir;
# adres degisince herkes bildirimleri yeniden acmali. Cloudflare bunu yalnizca deneme icin sunar.
# cloudflared ilk calistirmada Cloudflare'in resmi GitHub sayfasindan indirilir.

$ErrorActionPreference = "Stop"
$data = Join-Path $env:LOCALAPPDATA "PreBot-deneme"
New-Item -ItemType Directory -Force $data | Out-Null
$exe = Join-Path $data "cloudflared.exe"

if (-not (Test-Path $exe)) {
    $url = "https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-windows-amd64.exe"
    Write-Host "cloudflared indiriliyor: $url" -ForegroundColor Cyan
    Invoke-WebRequest -Uri $url -OutFile $exe -UseBasicParsing
    $signature = Get-AuthenticodeSignature $exe
    Write-Host ("Imza: " + $signature.Status + " - " + $signature.SignerCertificate.Subject) -ForegroundColor DarkGray
    if ($signature.Status -ne "Valid") {
        Remove-Item $exe
        Write-Host "Dosyanin imzasi gecerli degil, silindi." -ForegroundColor Red
        exit 1
    }
}

Write-Host "Tunel aciliyor. Asagida https://....trycloudflare.com adresi gorunecek." -ForegroundColor Green
Write-Host "Durdurmak icin Ctrl+C." -ForegroundColor DarkGray
& $exe tunnel --no-autoupdate --url http://localhost:5215
