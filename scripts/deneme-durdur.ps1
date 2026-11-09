# Arkadas denemesinin sitesini ve tunelini durdurur (deneme-baslat.ps1 ile acilanlar).
#   powershell -ExecutionPolicy Bypass -File .\scripts\deneme-durdur.ps1
param([switch]$Sessiz)

$data = Join-Path $env:LOCALAPPDATA "PreBot-deneme"
$stopped = 0

# The tunnel: cloudflared.exe from the test folder only.
Get-CimInstance Win32_Process -Filter "Name = 'cloudflared.exe'" |
    Where-Object { $_.ExecutablePath -eq (Join-Path $data "cloudflared.exe") } |
    ForEach-Object { Stop-Process -Id $_.ProcessId -Confirm:$false -ErrorAction SilentlyContinue; $stopped++ }

# The site: only the process id saved by deneme-sitesi.ps1, so no other PreBot copy is touched.
$pidFile = Join-Path $data "site.pid"
$sitePids = @()
if (Test-Path $pidFile) { $sitePids += [int](Get-Content $pidFile -Raw).Trim() }
foreach ($procId in $sitePids) {
    $p = Get-CimInstance Win32_Process -Filter "ProcessId = $procId"
    if ($p -and $p.CommandLine -like "*PreBot.Web.dll*") {
        Stop-Process -Id $procId -Confirm:$false -ErrorAction SilentlyContinue
        $stopped++
    }
}
Remove-Item $pidFile -ErrorAction SilentlyContinue

# Tailscale Funnel (if it was used): stop sharing the site publicly. Only the public HTTPS share is turned
# off; any other Tailscale serve settings on this computer are left alone.
$ts = (Get-Command tailscale -ErrorAction SilentlyContinue).Source
if (-not $ts) { $ts = Join-Path $env:ProgramFiles "Tailscale\tailscale.exe" }
if ((Test-Path $ts) -and -not $Sessiz) {
    & $ts funnel --https=443 off 2>$null | Out-Null
}

if (-not $Sessiz) {
    Write-Host ("Durduruldu: " + $stopped + " islem.") -ForegroundColor Green
}
