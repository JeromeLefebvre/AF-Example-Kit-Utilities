using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Analysis;
using System.Data;
using OSIsoft.AF.UnitsOfMeasure;

namespace ConvertUOMs
{
    class Program
    {
        static public DataTable dt;
        static public PISystem system;
        static void Main(string[] args)
        {
            system = new PISystems().DefaultPISystem;
            var db = system.Databases.DefaultDatabase;
            convertdb(db);
        }

        public static void convertdb(AFDatabase db)
        {
            dt = db.Tables["UOM Conversion"].Table;
            convertAllAttributes(db);
        }

        static void convertAllAttributes(AFDatabase db)
        {
            foreach (var elem in db.ElementTemplates)
                foreach (var attr in elem.AttributeTemplates)
                    convertAttribute(attr);
            db.CheckIn();
        }

        static void convertAttribute(AFAttributeTemplate attr)
        {
            foreach (var child in attr.AttributeTemplates)
                convertAttribute(child);

            attr.DefaultUOM = convert(attr.DefaultUOM);
            if (attr.DataReferencePlugIn.Name == "PI Point")
                attr.DataReference.UOM = convert(attr.DataReference.UOM);

            if (attr.DataReferencePlugIn.Name == "Table Lookup")
            {
                // Create a table look object and use its method to grab the uom
            }
                

            attr.ElementTemplate.CheckIn();
        }
        static UOM convert(UOM initialUOM, string kitName = "Japan")
        {
            try
            {
                DataRow[] result = dt.Select($"UOM  = '{initialUOM.Abbreviation}'");
                return system.UOMDatabase.UOMs[(string)result[0][kitName]];
            }
            catch (Exception e)
            {
                return initialUOM;
            }
        }
    }
}
