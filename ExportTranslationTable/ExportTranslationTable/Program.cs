using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.Asset;

namespace ExportTranslationTable
{
    class Program
    {
        static void Main(string[] args)
        {
            PISystem local = new PISystems().DefaultPISystem;
            AFDatabase db = local.Databases.DefaultDatabase;
            AFTable translation = db.Tables["Translations"];
            local.ExportXml(translation, PIExportMode.AllReferences, @"..\translation.xml", null, null, null);
        }
    }
}
