# Arkadaslarla kapali deneme icin siteyi BU BILGISAYARDA "yayin modunda" baslatir.
#   powershell -ExecutionPolicy Bypass -File .\scripts\deneme-sitesi.ps1
# Sonra ikinci bir pencerede tuneli ac (internetten erisilsin diye):
#   powershell -ExecutionPolicy Bypass -File .\scripts\deneme-tuneli.ps1
#
# - Giris: giris-ayarla.ps1 ile Google anahtari kaydedildiyse "Google ile giris" (anahtar ekranda gorunmez,
#   dosyaya yazilmaz). Kaydedilmediyse ya da -DavetKodu verilirse "davet kodu + isim + sifre".
# - Deneme sitesi 5290 portunda calisir; gelistirme (dotnet run) 5215'te kalir, tunel ona hic baglanmaz.
# - Veriler %LOCALAPPDATA%\PreBot-deneme klasorunde, gelistirme verilerinden ayri.
# - Discord botu kapali (denemede Discord'a hicbir sey gonderilmez).
param(
    [string]$Kod,
    # Your user number (Profil sayfasinda yazar), so you can open /yonetim. Kept for later runs.
    [string]$Yonetici,
    # Keep "name + invite code" sign-in even when Google sign-in is set up.
    [switch]$DavetKodu
)

$ErrorActionPreference = "Stop"
$root = Resolve-Path "$PSScriptRoot\.."
$data = Join-Path $env:LOCALAPPDATA "PreBot-deneme"
$app = Join-Path $data "app"
New-Item -ItemType Directory -Force $data | Out-Null

# A random code that cannot be guessed (old 4-digit codes are replaced). No look-alike letters (0/o, 1/l/i).
function New-InviteCode {
    $alphabet = "abcdefghjkmnpqrstuvwxyz23456789"
    $rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
    $buffer = New-Object byte[] 1
    $chars = @()
    while ($chars.Count -lt 12) {
        $rng.GetBytes($buffer)
        if ($buffer[0] -lt (256 - (256 % $alphabet.Length))) { $chars += $alphabet[$buffer[0] % $alphabet.Length] }
    }
    return "flex-" + (-join $chars[0..3]) + "-" + (-join $chars[4..7]) + "-" + (-join $chars[8..11])
}

# Google sign-in keys saved by giris-ayarla.ps1 (user-secrets of PreBot.Web). Production mode does not read
# user-secrets by itself, so they are handed to the site process here. Never printed.
$google = @{}
foreach ($line in (dotnet user-secrets list --project "$root\src\PreBot.Web" 2>$null)) {
    if ($line -match '^Authentication:Google:(ClientId|ClientSecret) = (.+)$') { $google[$Matches[1]] = $Matches[2].Trim() }
}
$useGoogle = $google.ClientId -and $google.ClientSecret
$useInvite = $DavetKodu -or $Kod -or -not $useGoogle
Set-Content -Path (Join-Path $data "giris.txt") -Value $(if ($useInvite) { "kod" } else { "google" }) -Encoding ascii

$codeFile = Join-Path $data "davet-kodu.txt"
if ($Kod) {
    Set-Content -Path $codeFile -Value $Kod -Encoding ascii
} elseif (Test-Path $codeFile) {
    $Kod = (Get-Content $codeFile -Raw).Trim()
}
if (-not $Kod -or $Kod.Length -lt 12) {
    $Kod = New-InviteCode
    Set-Content -Path $codeFile -Value $Kod -Encoding ascii
}

$adminFile = Join-Path $data "yonetici.txt"
$Yonetici = ("" + $Yonetici).Replace("#", "").Trim()
if ($Yonetici) {
    Set-Content -Path $adminFile -Value $Yonetici -Encoding ascii
} elseif (Test-Path $adminFile) {
    $Yonetici = (Get-Content $adminFile -Raw).Trim()
}

Write-Host "Site derleniyor (yayin modu)..." -ForegroundColor Cyan
dotnet publish "$root\src\PreBot.Web\PreBot.Web.csproj" -c Release -o $app --nologo -v quiet
if ($LASTEXITCODE -ne 0) { Write-Host "Derleme basarisiz." -ForegroundColor Red; exit 1 }

$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:ASPNETCORE_URLS = "http://localhost:5290"
$env:DataDirectory = $data
$env:Site__InviteCode = $(if ($useInvite) { $Kod } else { "" })
if ($useGoogle) {
    $env:Authentication__Google__ClientId = $google.ClientId
    $env:Authentication__Google__ClientSecret = $google.ClientSecret
}
Remove-Variable google
$env:Site__OperatorName = "PreBot deneme grubu"
$env:Site__HostingLocation = "Turkiye (gelistiricinin bilgisayari, gecici deneme)"
$env:Discord__Enabled = "false"
$env:Backup__Enabled = "false"
$env:Site__AdminUserIds = $Yonetici
# The site is only reached through an HTTPS tunnel (Cloudflare / Tailscale): always build https:// links.
$env:ReverseProxy__AssumeHttps = "true"

Write-Host ""
if ($useGoogle) { Write-Host "Giris: Google ile" -ForegroundColor Green }
if ($useInvite) { Write-Host "Davet kodu: $Kod" -ForegroundColor Green }
Write-Host "Site: http://localhost:5290  (internetten erisim icin deneme-tuneli.ps1'i ac)" -ForegroundColor Green
Write-Host "Durdurmak icin Ctrl+C." -ForegroundColor DarkGray
Write-Host ""

# The process id is saved so deneme-durdur.ps1 stops exactly this site and nothing else.
# The site's own logs (JSON lines) go to uygulama.log / uygulama-hata.log in the test folder.
Write-Host ("Kayitlar: " + (Join-Path $data "uygulama.log")) -ForegroundColor DarkGray
$proc = Start-Process dotnet -ArgumentList ".\PreBot.Web.dll" -WorkingDirectory $app -NoNewWindow -PassThru `
    -RedirectStandardOutput (Join-Path $data "uygulama.log") -RedirectStandardError (Join-Path $data "uygulama-hata.log")
Set-Content -Path (Join-Path $data "site.pid") -Value $proc.Id -Encoding ascii
Wait-Process -Id $proc.Id
