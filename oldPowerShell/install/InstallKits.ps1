$afimport = $env:PIHOME64 + 'AF\AFImport.exe'
$file = "C:\GitHub\scripts\kits\Original\Asset_Based_PI_Example_Kit_Utilities_Cost_Management_v2016A\OSIDemo_FULL_UtilitiesManagementSystem_v2016A.xml" 


Get-ChildItem "C:\GitHub\scripts\kits\Original\Asset_Based_PI_Example_Kit_Utilities_Cost_Management_v2016A" -Filter UOM* |
Foreach-Object {
    $file = $_.FullName
    &$afimport \\localhost\kit4 /file:$file 2> $_$err.txt
}

Get-ChildItem "C:\GitHub\scripts\kits\Original\Asset_Based_PI_Example_Kit_Utilities_Cost_Management_v2016A" -Filter OSIDemo_FULL* |
Foreach-Object {
    $file = $_.FullName
    &$afimport \\localhost\kit4 /file:$file
}
