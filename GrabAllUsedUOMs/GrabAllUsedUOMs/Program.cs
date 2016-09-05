using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.UnitsOfMeasure;
using System.Data;
using OSIsoft.AF.Analysis;
using System.Text.RegularExpressions;

namespace GrabAllUsedUOMs
{
    class Program
    {
        static void Main(string[] args)
        {
            PISystem system = new PISystems().DefaultPISystem;
            AFDatabase db = system.Databases.DefaultDatabase;
            getAllUOMUsed(db);
            db.CheckIn();
        }

        static AFTable createUOMTable(AFDatabase db)
        {
            AFTable table = db.Tables["UOM Conversion"];
            // TODO: IF it does not exist, write code to create it.
            return table;
        }
        static void getAllUOMUsed(AFDatabase db)
        {
            DataTable dt = createUOMTable(db).Table;
            foreach (var elem in db.ElementTemplates)
                getAllUOMUsed(elem, dt);
            foreach (AFAnalysisTemplate analysis in db.AnalysisTemplates)
                getAllUOMUsed(analysis, dt);
        }
        static void getAllUOMUsed(AFAnalysisTemplate analysis, DataTable dt)
        {
            List<string> uoms = new List<string>();
            if (analysis.AnalysisRulePlugIn.Name == "PerformanceEquation")
                parseForAllUOMs(analysis.AnalysisRule.ConfigString, ref uoms);
            
            //foreach(var rule in analysis.AnalysisRule)
        }

        static void parseForAllUOMs(string expresion, ref List<string> uoms)
        {
            if (!expresion.Contains("Convert"))
                return;
            Regex findFirstConvert = new Regex(@"Convert\(((?<BR>\()|(?<-BR>\))|[^()]*)+\)");
            var match = findFirstConvert.Match(expresion);
            var firstConvert = match.Value;

            // Find the UOM in the convert that was found


            var UOMraw = firstConvert.Split(',').Last().Trim(')').TrimStart();
            UOMraw = UOMraw.TrimStart('\"');
            UOMraw = UOMraw.TrimEnd('\"');
            if (!uoms.Contains(UOMraw))
                uoms.Add(UOMraw);

            var firstArgument = firstConvert.Substring(8);
            if (firstArgument.Contains("Convert"))
                parseForAllUOMs(firstArgument, ref uoms);
            // look for UOMs in the first argument and in the last argument
            var remainder = expresion.Substring(match.Index + firstConvert.Length);
            if (remainder.Contains("Convert"))
                parseForAllUOMs(remainder, ref uoms);

        }
        static void getAllUOMUsed(AFElementTemplate elem, DataTable dt)
        {
            foreach (var attr in elem.AttributeTemplates)
                getAllUOMUsed(attr, dt);

        }
        static void getAllUOMUsed(AFAttributeTemplate attr, DataTable dt)
        {
            foreach (var child in attr.AttributeTemplates)
                getAllUOMUsed(child, dt);
            if (attr.DefaultUOM != null)
                insert(attr.DefaultUOM, dt);
            if (attr.DataReference != null && attr.DataReference.UOM != null)
                insert(attr.DataReference.UOM, dt);
        }

        static void insert(UOM uom, DataTable dt)
        {
            if (!dt.AsEnumerable().Any(row => uom.Abbreviation == row.Field<String>("UOM")))
            {
                DataRow row = dt.NewRow();
                row["UOM"] = uom.Abbreviation;
                row["Japan"] = uom.Class.CanonicalUOM.Abbreviation;
                dt.Rows.Add(row);
            }
        }
    }
}
