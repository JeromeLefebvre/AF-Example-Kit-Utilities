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
            AFTable uomconversion = db.Tables["UOM Conversion"];
            local.ExportXml(uomconversion, PIExportMode.AllReferences, @"..\uomConversion.xml", null, null, null);
        }
    }
}
