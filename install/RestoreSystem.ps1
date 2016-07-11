# Delete all PI Tags created
$conn = Connect-PIDataArchive -PIDataArchiveMachineName localhost
$tags = Get-PIPoint -Name OSIDEMO_* -Connection $conn
Write-Host $tags
foreach ($tag in $tags) {
    Write-Host "Deleting " $tag.Point.Name
    Remove-PIPoint -Connection $conn -Name $tag.Point.Name
}

# Restore the AF Server
net stop AFService
sqlcmd -E -S LocalHost\sqlexpress -Q "Restore Database [PIFD] From DISK='C:\SQLBACKUP\pifd'"
net start AFService