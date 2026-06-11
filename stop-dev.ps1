# Stops running dev processes that lock build output DLLs.
$names = @('CleanArchitecture.Web', 'CleanArchitecture.AppHost', 'dcp')
foreach ($name in $names) {
    Get-Process -Name $name -ErrorAction SilentlyContinue | Stop-Process -Force
}

Get-NetTCPConnection -LocalPort 44447 -ErrorAction SilentlyContinue |
    ForEach-Object { Stop-Process -Id $_.OwningProcess -Force -ErrorAction SilentlyContinue }

Write-Host 'Dev processes stopped. You can build again.'
