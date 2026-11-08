# Google / Discord login keys -> user-secrets of PreBot.Web. The secret is never shown on screen or written to a file in the repo.
# Run: powershell -ExecutionPolicy Bypass -File .\scripts\giris-ayarla.ps1
$project = Join-Path $PSScriptRoot "..\src\PreBot.Web"

Write-Host "Hangi giris yontemini ayarlamak istiyorsun?"
Write-Host "  1) Google"
Write-Host "  2) Discord"
$choice = Read-Host "Secim (1/2)"

switch ($choice) {
    "1" { $provider = "Google"; $defaultId = "" }
    "2" { $provider = "Discord"; $defaultId = "1569059695430209727" }
    default { Write-Host "Gecersiz secim." -ForegroundColor Yellow; exit 1 }
}

$prompt = "$provider Client ID"
if ($defaultId) { $prompt += " (Enter = $defaultId)" }
$clientId = (Read-Host $prompt).Trim()
if (-not $clientId) { $clientId = $defaultId }
if (-not $clientId) { Write-Host "Client ID bos olamaz." -ForegroundColor Yellow; exit 1 }

$secure = Read-Host "$provider Client Secret (ekranda gorunmez, Ctrl+V ile yapistir)" -AsSecureString
$secret = [System.Net.NetworkCredential]::new('', $secure).Password.Trim()
if (-not $secret) { Write-Host "Secret bos geldi, hicbir sey kaydedilmedi." -ForegroundColor Yellow; exit 1 }

dotnet user-secrets set "Authentication:${provider}:ClientId" $clientId --project $project | Out-Null
dotnet user-secrets set "Authentication:${provider}:ClientSecret" $secret --project $project | Out-Null
$ok = $LASTEXITCODE -eq 0
Remove-Variable secret, secure

if ($ok) {
    Write-Host "$provider girisi kaydedildi. Siteyi yeniden baslat; giris sayfasinda '$provider ile giris' butonu cikacak." -ForegroundColor Green
} else {
    Write-Host "Kaydedilemedi." -ForegroundColor Red
    exit 1
}
