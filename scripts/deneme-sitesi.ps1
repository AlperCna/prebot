# Arkadaslarla kapali deneme icin siteyi BU BILGISAYARDA "yayin modunda" baslatir.
#   powershell -ExecutionPolicy Bypass -File .\scripts\deneme-sitesi.ps1
# Sonra ikinci bir pencerede tuneli ac (internetten erisilsin diye):
#   powershell -ExecutionPolicy Bypass -File .\scripts\deneme-tuneli.ps1
#
# - Giris "isim + davet kodu" ile (Google ayari gerekmez). Kod ilk calistirmada uretilir ve saklanir.
# - Veriler %LOCALAPPDATA%\PreBot-deneme klasorunde, gelistirme verilerinden ayri.
# - Discord botu kapali (denemede Discord'a hicbir sey gonderilmez).
param(
    [string]$Kod
)

$ErrorActionPreference = "Stop"
$root = Resolve-Path "$PSScriptRoot\.."
$data = Join-Path $env:LOCALAPPDATA "PreBot-deneme"
$app = Join-Path $data "app"
New-Item -ItemType Directory -Force $data | Out-Null

$codeFile = Join-Path $data "davet-kodu.txt"
if ($Kod) {
    Set-Content -Path $codeFile -Value $Kod -Encoding ascii
} elseif (Test-Path $codeFile) {
    $Kod = (Get-Content $codeFile -Raw).Trim()
} else {
    $Kod = "flex-" + (Get-Random -Minimum 1000 -Maximum 9999)
    Set-Content -Path $codeFile -Value $Kod -Encoding ascii
}

Write-Host "Site derleniyor (yayin modu)..." -ForegroundColor Cyan
dotnet publish "$root\src\PreBot.Web\PreBot.Web.csproj" -c Release -o $app --nologo -v quiet
if ($LASTEXITCODE -ne 0) { Write-Host "Derleme basarisiz." -ForegroundColor Red; exit 1 }

$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:ASPNETCORE_URLS = "http://localhost:5215"
$env:DataDirectory = $data
$env:Site__InviteCode = $Kod
$env:Site__OperatorName = "PreBot deneme grubu"
$env:Site__HostingLocation = "Turkiye (gelistiricinin bilgisayari, gecici deneme)"
$env:Discord__Enabled = "false"
$env:Backup__Enabled = "false"

Write-Host ""
Write-Host "Davet kodu: $Kod" -ForegroundColor Green
Write-Host "Site: http://localhost:5215  (internetten erisim icin deneme-tuneli.ps1'i ac)" -ForegroundColor Green
Write-Host "Durdurmak icin Ctrl+C." -ForegroundColor DarkGray
Write-Host ""

# The process id is saved so deneme-durdur.ps1 stops exactly this site and nothing else.
$proc = Start-Process dotnet -ArgumentList ".\PreBot.Web.dll" -WorkingDirectory $app -NoNewWindow -PassThru
Set-Content -Path (Join-Path $data "site.pid") -Value $proc.Id -Encoding ascii
Wait-Process -Id $proc.Id
