using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.Asset;

namespace DuplicateDefaultDB
{
    class Program
    {
        static void Main(string[] args)
        {
            var systems = new PISystems();
            var system = systems.DefaultPISystem;
            var olddb = system.Databases.DefaultDatabase;
            var xml = system.ExportXml(olddb, PIExportMode.AllReferences);
            var newdb = system.Databases.Add(olddb.Name + " " + DateTime.Now.ToString("hh:mm"));
            system.ImportXml(newdb, PIImportMode.PasteOperation, xml);
            system.Databases.DefaultDatabase = newdb;
            newdb.CheckIn();
            system.CheckIn();
        }
    }
}
