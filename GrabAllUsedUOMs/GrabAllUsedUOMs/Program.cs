using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text.RegularExpressions;

using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.UnitsOfMeasure;
using OSIsoft.AF.Analysis;

using CommandLine;
using CommandLine.Text;


namespace GrabAllUsedUOMs
{
    class Program
    {
        class Options
        {
            [Option('u', "UOMGrouping", Required = false, DefaultValue = "Japan",
              HelpText = "The UOM Grouping to use")]
            public string uomgrouping { get; set; }

            [ParserState]
            public IParserState LastParserState { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                  (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        public static string uomgrouping;
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                // Command values are available here
                uomgrouping = options.uomgrouping;
            }
            PISystem system = new PISystems().DefaultPISystem;
            AFDatabase db = system.Databases.DefaultDatabase;
            getAllUOMUsed(db);
            db.CheckIn();
        }

        static AFTable createUOMTable(AFDatabase db)
        {
            AFTable table = db.Tables["UOM Groupings"];
            if (table == null)
            {
                table = db.Tables.Add("UOM Groupings");
                DataTable datatable = new DataTable();
                datatable.Columns.Add("UOM", typeof(System.String));
                datatable.Columns.Add(uomgrouping, typeof(System.String));
                table.Table = datatable;

                db.CheckIn();
            }
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
            if (!dt.AsEnumerable().Any(row => uom.Abbreviation == row.Field<String>("Original")))
            {
                DataRow row = dt.NewRow();
                row["UOM"] = uom.Abbreviation;
                row[uomgrouping] = uom.Class.CanonicalUOM.Abbreviation;
                dt.Rows.Add(row);
            }
        }
    }
}
