# Arkadas denemesinin sitesini ve tunelini durdurur (deneme-baslat.ps1 ile acilanlar).
#   powershell -ExecutionPolicy Bypass -File .\scripts\deneme-durdur.ps1
param([switch]$Sessiz)

$data = Join-Path $env:LOCALAPPDATA "PreBot-deneme"
$stopped = 0

# The tunnel: cloudflared.exe from the test folder only.
Get-CimInstance Win32_Process -Filter "Name = 'cloudflared.exe'" |
    Where-Object { $_.ExecutablePath -eq (Join-Path $data "cloudflared.exe") } |
    ForEach-Object { Stop-Process -Id $_.ProcessId -Confirm:$false -ErrorAction SilentlyContinue; $stopped++ }

# The site: the process id saved by deneme-sitesi.ps1, or (older runs) "dotnet .\PreBot.Web.dll".
$pidFile = Join-Path $data "site.pid"
$sitePids = @()
if (Test-Path $pidFile) { $sitePids += [int](Get-Content $pidFile -Raw).Trim() }
$sitePids += Get-CimInstance Win32_Process -Filter "Name = 'dotnet.exe'" |
    Where-Object { $_.CommandLine -like '*.\PreBot.Web.dll*' } | Select-Object -ExpandProperty ProcessId
foreach ($procId in ($sitePids | Select-Object -Unique)) {
    $p = Get-CimInstance Win32_Process -Filter "ProcessId = $procId"
    if ($p -and $p.CommandLine -like "*PreBot.Web.dll*") {
        Stop-Process -Id $procId -Confirm:$false -ErrorAction SilentlyContinue
        $stopped++
    }
}
Remove-Item $pidFile -ErrorAction SilentlyContinue

if (-not $Sessiz) {
    Write-Host ("Durduruldu: " + $stopped + " islem.") -ForegroundColor Green
}
