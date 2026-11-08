# Discord bot token'ini user-secrets'a kaydeder. Token ekranda gorunmez ve dosyaya yazilmaz.
# Calistirma: powershell -ExecutionPolicy Bypass -File .\scripts\token-ayarla.ps1

$secure = Read-Host "Discord bot tokenini yapistir (Ctrl+V veya sag tik; ekranda gorunmez) ve Enter'a bas" -AsSecureString
$token = [System.Net.NetworkCredential]::new('', $secure).Password.Trim()

if ([string]::IsNullOrWhiteSpace($token)) {
    Write-Host "Token bos geldi, hicbir sey kaydedilmedi. Tekrar dene." -ForegroundColor Yellow
    exit 1
}

dotnet user-secrets set "Discord:Token" $token --project "$PSScriptRoot\..\src\PreBot" | Out-Null
$ok = $LASTEXITCODE -eq 0
Remove-Variable token, secure

if ($ok) {
    Write-Host "Token kaydedildi. Artik botu baslatabilirsin." -ForegroundColor Green
} else {
    Write-Host "Token kaydedilemedi." -ForegroundColor Red
    exit 1
}
