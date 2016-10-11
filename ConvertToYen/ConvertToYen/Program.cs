using OSIsoft.AF;
using OSIsoft.AF.Analysis;
using OSIsoft.AF.Asset;
using OSIsoft.AF.UnitsOfMeasure;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace ConvertFromAmericanCurrency
{
    class Program
    {
        static public DataTable dt;
        static public PISystem system;
        static public UOMDatabase UOMdb;
        // The following arugment need to be read from an argument
        static public string kitName = "JP";
        static public double rate = 99.9;

        static void Main(string[] args)
        {
            system = new PISystems().DefaultPISystem;
            var db = system.Databases.DefaultDatabase;
            UOMdb = system.UOMDatabase;
            dt = db.Tables["Currency Conversion"].Table;
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

            var oldUOM = attr.DefaultUOM;
            var newUOM = convert(attr.DefaultUOM);
            var condition = attr.IsConfigurationItem && oldUOM != newUOM;

            var existingValues = new Dictionary<AFElement, Double>();
            if (condition)
            {
                var elements = attr.ElementTemplate.FindInstantiatedElements(true, AFSortField.Name, AFSortOrder.Ascending, Int32.MaxValue);
                foreach (var element in elements)
                {
                    var value = element.Attributes[attr.Name].GetValue().ValueAsDouble();

                    //element.Attributes[attr.Name].Data.UpdateValue(new AFValue(value * rate, oldUOM), OSIsoft.AF.Data.AFUpdateOption.Insert);
                    existingValues[(AFElement)element] = value * rate;
                }
                attr.Database.CheckIn();
            }
            attr.DefaultUOM = newUOM;
            attr.ElementTemplate.CheckIn();
            if (condition)
            {
                var elements = attr.ElementTemplate.FindInstantiatedElements(true, AFSortField.Name, AFSortOrder.Ascending, Int32.MaxValue);
                foreach (var element in elements)
                {
                    var newValue = existingValues[(AFElement)element];

                    element.Attributes[attr.Name].Data.UpdateValue(new AFValue(newValue, newUOM), OSIsoft.AF.Data.AFUpdateOption.Insert);
                }
                attr.Database.CheckIn();
            };


            //attr.DefaultUOM = convert(attr.DefaultUOM);

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
                DataRow[] result = dt.Select($"US  = '{initialUOM.Abbreviation}'");
                return system.UOMDatabase.UOMs[(string)result[0]["JP"]];
            }
            catch (Exception e)
            {
                return initialUOM;
            }
        }

        static UOMClass createUOM(PISystem system, string UOMClassName, string UOMCanonicalName, string UOMCanonicalAbbreviation)
        {
            UOMDatabase uomdb = system.UOMDatabase;
            UOMClass UOMClass = uomdb.UOMClasses[UOMClassName];
            if (UOMClass == null)
            {
                UOMClass = uomdb.UOMClasses.Add(UOMClassName, UOMCanonicalName, UOMCanonicalAbbreviation);
                system.CheckIn();
            }
            if (UOMClass.CanonicalUOM.Name != UOMCanonicalName)
            {
                throw new System.ArgumentException("The current existing UOM Classes has a different canonical UOM");
            }
            return UOMClass;
        }
    }
}
