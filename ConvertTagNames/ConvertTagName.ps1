
# Script to convert the tag naming convention from the old to the current system
<#
Namely, it should convert the following:
        <AFAttributeTemplate>
          <Name>Tagname</Name>
          <Type>String</Type>
          <Value type="String"></Value>
          <DataReference>String Builder</DataReference>
          <ConfigString>OSIDEMO_;%Element%.;%..|Attribute%.%ID%;</ConfigString>
        </AFAttributeTemplate>

to

        <AFAttributeTemplate>
          <Name>Descriptor</Name>
          <Type>String</Type>
          <Value type="String"></Value>
          <DataReference>String Builder</DataReference>
          <ConfigString>%Element%;%..|Attribute%;</ConfigString>
        </AFAttributeTemplate>
        <AFAttributeTemplate>
          <Name>Tagname</Name>
          <Type>String</Type>
          <Value type="String"></Value>
          <DataReference>String Builder</DataReference>
          <ConfigString>OSIDEMO_;.%..|AttributeID%;</ConfigString>
        </AFAttributeTemplate>


and also converts
        <ConfigString>\\%@\PI Data Archive|Name%\%@.|TagName%;UOM=kWh;ReadOnly=False;pointtype=Float32;pointsource=OSIDemo_AFAnalysis;exdesc=PI AF UMS Kit</ConfigString>
        to

        <ConfigString>\\%@\PI Data Archive|Name%\%@.|TagName%;UOM=$;ReadOnly=False;pointtype=Float32;pointsource=OSIDemo_AFAnalysis;exdesc=PI AF UMS Kit;descriptor=%@.|Descriptor%</ConfigString>

#>

$original = "C:\GitHub\scripts\kits\Original\Asset_Based_PI_Example_Kit_Utilities_Cost_Management_v2016A\OSIDemo_FULL_UtilitiesManagementSystem_v2016A.xml"
$modified = "C:\GitHub\scripts\kits\Modified\Asset_Based_PI_Example_Kit_Utilities_Cost_Management_v2016A\OSIDemo_FULL_UtilitiesManagementSystem_v2016A.xml"

# Find the OSIDemo_FULL file

# Add in the descriptor to each

# Add in the 

(Get-Content $original).replace('<ConfigString>OSIDEMO_;%Element%.;%..|Attribute%.%ID%;</ConfigString>', '<ConfigString>OSIDEMO_;.%..|AttributeID%;</ConfigString>') | Set-Content $modified
