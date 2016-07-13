[CmdletBinding()]
Param(
  [Parameter(Mandatory=$false,Position=1)]
   [string]$af = "",
	
   [Parameter(Mandatory=$false)]
   [string]$database = ""
)

$load = [Reflection.Assembly]::LoadWithPartialName("OSIsoft.AFSDK")  

if ($load) {
}
else {
    return "Unable to access the afsdk.dll"
}

function RefreshMapping([OSIsoft.AF.AFDatabase]$database) {
    Write-Host "Refreshing all mapping in the database: $database"
    foreach ($element in $database.ElementTemplates)
    {
        foreach ($analysis in $element.AnalysisTemplates)
        {
            $analysis.AnalysisRule.RefreshConfigurationAndVariableMapping()
        }
    }
    $database.CheckIn()
}

# Select the specified AF Server or pick the default one
if ($af -ne "") {
    $server = [OSIsoft.AF.PISystem]::CreatePISystem($af)
}
else {
    $server = (New-object 'OSIsoft.AF.PISystems').DefaultPISystem 
}

if ($server) {
}
else {
    return "The specified AF server, $af, does not exists or cannot be connected"
}

Write-Host "Locating at databases on the AF Server: $server"

# Refresh the specified database or refresh all of them
if ($database) {
    $db = $server.Databases[$database]
    if ($db) {
        RefreshMapping($db)
    }
    else {
        Write-Host "The specified AF database, $database, does not exists or cannot be read"
    }
}
else {
    foreach ($db in $server.Databases) {
        RefreshMapping($db)
    }
}

