# Discord bot token'ini user-secrets'a kaydeder. Token ekranda gorunmez ve dosyaya yazilmaz.
# Botu sitenin icinde calistirmak icin (onerilen):
#   powershell -ExecutionPolicy Bypass -File .\scripts\token-ayarla.ps1 -Proje web
# Botu tek basina calistirmak icin:
#   powershell -ExecutionPolicy Bypass -File .\scripts\token-ayarla.ps1
param(
    [ValidateSet("bot", "web")]
    [string]$Proje = "bot"
)

$project = if ($Proje -eq "web") { "$PSScriptRoot\..\src\PreBot.Web" } else { "$PSScriptRoot\..\src\PreBot" }

$secure = Read-Host "Discord bot tokenini yapistir (Ctrl+V veya sag tik; ekranda gorunmez) ve Enter'a bas" -AsSecureString
$token = [System.Net.NetworkCredential]::new('', $secure).Password.Trim()

if ([string]::IsNullOrWhiteSpace($token)) {
    Write-Host "Token bos geldi, hicbir sey kaydedilmedi. Tekrar dene." -ForegroundColor Yellow
    exit 1
}

dotnet user-secrets set "Discord:Token" $token --project $project | Out-Null
$ok = $LASTEXITCODE -eq 0
Remove-Variable token, secure

if ($ok) {
    Write-Host "Token kaydedildi ($Proje). Artik botu baslatabilirsin." -ForegroundColor Green
} else {
    Write-Host "Token kaydedilemedi." -ForegroundColor Red
    exit 1
}
