using OSIsoft.AF;

namespace ExportKitScript
{
    class Program
    {
        static void Main(string[] args)
        {
            var version = "v2016B";
            var pisystem = new PISystems().DefaultPISystem;
            foreach (var UOMClassName in new string[] { "Power", "Energy Cost", "Volume Cost", "US Currency", "Energy per Volume" }) {
                var UOMClass = pisystem.UOMDatabase.UOMClasses[UOMClassName];
                pisystem.ExportXml(UOMClass, PIExportMode.NoUniqueID, $"..\\UOM_{UOMClass.Name}_{version}.xml", null, null, null);
            }

            var db = pisystem.Databases.DefaultDatabase;
            pisystem.ExportXml(db, PIExportMode.NoUniqueID, $"..\\OSIDemo_Utilities Management System_{version}.xml", null, null, null);
        }
    }
}