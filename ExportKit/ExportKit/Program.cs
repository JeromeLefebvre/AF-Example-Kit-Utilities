using OSIsoft.AF;
using System.IO.Compression;

namespace ExportKitScript
{
    class Program
    {
        static void Main(string[] args)
        {
            var version = "v2016B";
            var pisystem = new PISystems().DefaultPISystem;
            var db = pisystem.Databases.DefaultDatabase;
            var directoryPath = $"..\\{db.Name}";

            System.IO.Directory.CreateDirectory(directoryPath);

            pisystem.ExportXml(db, PIExportMode.NoUniqueID, $"{directoryPath}\\OSIDemo_Utilities Management System_{version}.xml", null, null, null);

            foreach (var UOMClassName in new string[] { "Power", "Energy Cost", "Volume Cost", "US Currency", "Energy per Volume" }) {
                var UOMClass = pisystem.UOMDatabase.UOMClasses[UOMClassName];
                pisystem.ExportXml(UOMClass, PIExportMode.NoUniqueID, $"{directoryPath}\\UOM_{UOMClass.Name}_{version}.xml", null, null, null);
            }

            ZipFile.CreateFromDirectory(directoryPath, $"..\\{db.Name}.zip", CompressionLevel.Optimal, false);
        }
    }
}