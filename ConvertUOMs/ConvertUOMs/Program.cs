using System;
using System.Data;
using System.Text.RegularExpressions;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Analysis;
using OSIsoft.AF.UnitsOfMeasure;

namespace ConvertUOMs
{
    class Program
    {
        static public DataTable dt;
        static public PISystem system;
        static public UOMDatabase UOMdb;
        // This needs to be read from an argument
        static public string kitName = "Japan";
        static void Main(string[] args)
        {
            system = new PISystems().DefaultPISystem;
            var db = system.Databases.DefaultDatabase;
            UOMdb = system.UOMDatabase;
            dt = db.Tables["UOM Conversion"].Table;
            convertAttributesAndAnalysis(db);
        }
        static void convertAttributesAndAnalysis(AFDatabase db)
        {
            foreach (var elem in db.ElementTemplates)
                foreach (var attr in elem.AttributeTemplates)
                    convertAttribute(attr);

            foreach (var analysis in db.AnalysisTemplates)
                if (analysis.AnalysisRulePlugIn.Name == "PerformanceEquation")
                    convertAnalysis(analysis);
            db.CheckIn();
        }

        static void convertAnalysis(AFAnalysisTemplate analysis)
        {
            var configString = analysis.AnalysisRule.ConfigString;

            foreach (Match match in Regex.Matches(configString, "\"(?<uom>.*?)\""))
            {

                var oldUOM = UOMdb.UOMs[match.Groups["uom"].Value];
                // We differentiate if what is found is an UOM by checking if it exists in the UOM database
                if (oldUOM == null)
                    continue;
                var newUOM = convert(oldUOM);
                configString = configString.Replace(match.Groups["uom"].Value, newUOM.Abbreviation);
            }
            analysis.AnalysisRule.ConfigString = configString;
            analysis.CheckIn();
        }
        static void convertAttribute(AFAttributeTemplate attr)
        {
            foreach (var child in attr.AttributeTemplates)
               convertAttribute(child);

            try
            {
                attr.DefaultUOM = convert(attr.DefaultUOM);
            }
            // occurs for example when trying to change the UOMs of Location attributes.
            catch(System.InvalidOperationException)
            {

            }
            if (attr.DataReferencePlugIn != null)
            {
                if (attr.DataReferencePlugIn.Name == "PI Point")
                    attr.DataReference.UOM = convert(attr.DataReference.UOM);

                if (attr.DataReferencePlugIn.Name == "Table Lookup")
                {
                    // Create a table look object and use its method to grab the uom
                }
            }
            attr.ElementTemplate.CheckIn();
        }
        static UOM convert(UOM initialUOM)
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
