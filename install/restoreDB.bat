net stop AFService

sqlcmd -E -S LocalHost\sqlexpress -Q "Restore Database [PIFD] From DISK='C:\SQLBACKUP\pifd'"

net start AFService