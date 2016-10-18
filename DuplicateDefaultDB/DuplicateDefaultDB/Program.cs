using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using CommandLine;
using CommandLine.Text;

namespace DuplicateDefaultDB
{
    class Program
    {
        class Options
        {
            [Option('p', "postfix", Required = true, DefaultValue = "Japan",
              HelpText = "A postfix for the database")]
            public string suffix { get; set; }

            [Option('t', "time", Required = false, DefaultValue = true,
            HelpText = "A postfix for the database")]
            public Boolean timesuffix { get; set; }

            [ParserState]
            public IParserState LastParserState { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                  (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }
        public static string suffix = "Japan";

        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                // Command values are available here
                suffix = options.suffix;
                var systems = new PISystems();
                var system = systems.DefaultPISystem;
                var olddb = system.Databases.DefaultDatabase;
                var xml = system.ExportXml(olddb, PIExportMode.AllReferences);
                AFDatabase newdb;
                if (options.timesuffix)
                    newdb = system.Databases.Add(olddb.Name + " " + suffix + " " + DateTime.Now.ToString("mm"));
                else
                    newdb = system.Databases.Add(olddb.Name + " " + suffix);
                system.ImportXml(newdb, PIImportMode.PasteOperation, xml);
                system.Databases.DefaultDatabase = newdb;
                newdb.CheckIn();
                system.CheckIn();
            }
        }
    }
}
