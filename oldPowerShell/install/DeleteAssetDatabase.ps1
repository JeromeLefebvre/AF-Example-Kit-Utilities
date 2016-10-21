$afserver = Get-AFServer -Name localhost
$conn = Connect-AFServer -AFServer $afserver
$afdatabases = Get-AFDatabase -AFServer $conn -Name Asset*
foreach ($afdatabase in $afdatabases) {
    Write-Host "Deleting" $afdatabase
    $status = Remove-AFDatabase -AFDatabase $afdatabase
}