using CommandLine;
using CommandLine.Text;
using OSIsoft.AF;
using System;
using System.IO.Compression;
using OSIsoft.AF.UnitsOfMeasure;

namespace ExportKitScript
{
    class Program
    {
        class Options
        {

            [Option('c', "currency", Required = false, DefaultValue = "JPY",
              HelpText = "The country code for the currency us, JPY, CAD, etc. http://www.xe.com/symbols.php")]
            public string currency { get; set; }

            [Option('u', "uomgrouping", Required = false, DefaultValue = "Japan",
              HelpText = "")]
            public string uomgrouping { get; set; }

            [Option('l', "language", Required = false, DefaultValue = "ja",
              HelpText = "")]
            public string language { get; set; }


            [ParserState]
            public IParserState LastParserState { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                  (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }
        public static string currency = "KRW";
        public static string language = "ja";
        public static string uomgrouping = "Japan";

        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                // Command values are available here
                currency = options.currency;
            }
            var version = "v2016B";
            var pisystem = new PISystems().DefaultPISystem;
            var db = pisystem.Databases.DefaultDatabase;
            var directoryPath = $"..\\{db.Name}";

            try
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            pisystem.ExportXml(db, PIExportMode.NoUniqueID, $"{directoryPath}\\OSIDemo_Utilities Management System_{version}.xml", null, null, null);

            UOMClass uomclass;
            try
            {
                foreach (var UOMClassName in new string[] { "Power", $"Energy Cost - {currency} Currency", $"Volume Cost - {currency} Currency", "Volume Flow Rate", $"{currency} Currency", $"Energy per Volume" })
                {
                    uomclass = pisystem.UOMDatabase.UOMClasses[UOMClassName];
                    if (uomclass == null)
                        Console.WriteLine($"Could not find: {UOMClassName}");
                    else
                        pisystem.ExportXml(uomclass, PIExportMode.NoUniqueID, $"{directoryPath}\\UOM_{uomclass.Name}_{version}.xml", null, null, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            ZipFile.CreateFromDirectory(directoryPath, $"..\\{db.Name}_{uomgrouping}_{language}_{currency}.zip", CompressionLevel.Optimal, false);

        }
    }
}