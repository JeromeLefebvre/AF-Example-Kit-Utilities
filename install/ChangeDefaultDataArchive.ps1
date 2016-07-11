
[System.Reflection.Assembly]::LoadWithPartialName("OSIsoft.AFSDK")
[OSIsoft.AF.PISystems] $pisystems = New-Object OSIsoft.AF.PISystems

[OSIsoft.AF.PISystem] $pisystem = $pisystems["localhost"]
[OSIsoft.AF.AFDatabase] $db = $pisystem.Databases["Kit"]
[OSIsoft.AF.Asset.AFElement] $el = $db.Elements["PI Data Archive"]
[OSIsoft.AF.Asset.AFAttribute] $attr = $el.attributes["Name"]

$attr.Data.UpdateValue("localhost", [OSIsoft.AF.Data.AFUpdateOption]::Insert)