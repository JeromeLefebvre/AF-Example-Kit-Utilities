using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

using OSIsoft.AF.Asset;
using OSIsoft.AF;
using OSIsoft.AF.Analysis;

using Google.Apis.Translate.v2;
using Google.Apis.Translate.v2.Data;
using Google.Apis.Services;
using static Google.Apis.Translate.v2.TranslationsResource;

using CommandLine;
using CommandLine.Text;

namespace AllNamesAndDescriptions
{
    class Program
    {

        class Options
        {
            [Option('l', "language", Required = true, DefaultValue = "ko",
              HelpText = "The language to translate to")]
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

        public static string keyPath = @"c:\apikey.txt";
        public static string language;


        public static HashSet<string> usedWords = new HashSet<string>();
        private static TranslateService service = new TranslateService(new BaseClientService.Initializer()
        {
            ApiKey = File.ReadAllText(keyPath), // your API key, that you get from Google Developer Console
            ApplicationName = "jlefebvrenew" // your application name, that you get form Google Developer Console
        });
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                // Command values are available here
                language = options.language;
            }
            PISystem system = new PISystems().DefaultPISystem;
            AFDatabase kitdb = system.Databases.DefaultDatabase;
            fillEnglishTable(kitdb);

            kitdb.CheckIn();
        }

        public static string translate(string english, string language)
        {
            string[] srcText = new[] { english };
            ListRequest request = service.Translations.List(srcText, language);
            request.Source = "en";
            TranslationsListResponse response = request.Execute();
            String translated_text = response.Translations[0].TranslatedText;
            return translated_text;
        }

        static void addCodeComments(AFAnalysisTemplate analysis, DataTable dt)
        {
            string configString = analysis.AnalysisRule.ConfigString;
            Regex rgx = new Regex(@"\/\*(?<comment>.*?)\*\/");
            foreach (Match match in Regex.Matches(configString, @"\/\*(?<comment>.*?)\*\/"))
                insert(dt, match.Groups["comment"].Value);
        }

        static void fillEnglishTable(AFDatabase db)
        {
            AFTable translation = createOrReturnTranslationTable(db);
            DataTable dt = translation.Table;

            insert(dt, db.Description);
            foreach (AFElement element in db.Elements)
                storeAllElement(element, dt);
            db.CheckIn();

            foreach (AFTable table in db.Tables)
                if (!table.Name.StartsWith("Translations_")  && table.Name != "UOM Groupings" && table.Name != "Currency Conversion")
                    storeAllTableHeaders(table, dt);

            db.CheckIn();
            foreach (AFElementTemplate elem in db.ElementTemplates)
            {
                addNameAndDescription(dt, elem, elem);
                insert(dt, elem.NamingPattern);
            }
            foreach (var analysis in db.AnalysisTemplates)
            {
                addNameAndDescription(dt, analysis, analysis);
                if (analysis.AnalysisRulePlugIn.Name == "PerformanceEquation")
                    addCodeComments(analysis, dt);
            }
            db.CheckIn();
            foreach (AFCategory category in db.AnalysisCategories)
                addNameAndDescription(dt, category, category);
            foreach (AFCategory category in db.ElementCategories)
                addNameAndDescription(dt, category, category);
            foreach (AFCategory category in db.AttributeCategories)
                addNameAndDescription(dt, category, category);
            foreach (AFCategory category in db.TableCategories)
                addNameAndDescription(dt, category, category);
            foreach (AFCategory category in db.ReferenceTypeCategories)
                addNameAndDescription(dt, category, category);
            db.CheckIn();

            foreach (var enumSet in db.EnumerationSets)
                storeEnumerationSet(enumSet, dt);

            removeUnused(dt);
        }

        static void removeUnused(DataTable dt)
        {
            var results = dt.AsEnumerable().Where(i => !usedWords.Contains(i["en"]));
            Console.WriteLine("The following lines are in the database but not used");
            foreach (var result in results)
                Console.WriteLine(result.ItemArray.GetValue(0));
        }

        static void storeEnumerationSet(AFEnumerationSet enumSet, DataTable dt)
        {
            addNameAndDescription(dt, enumSet, enumSet);
            foreach (var row in enumSet)
                addNameAndDescription(dt, row, row);
        }

        static void storeAllTableHeaders(AFTable table, DataTable dt)
        {
            addNameAndDescription(dt, table, table);
            foreach (DataColumn column in table.Table.Columns)
                insert(dt, column.ColumnName);
            foreach (DataRow row in table.Table.Rows)
                foreach (var entry in row.ItemArray)
                    if (entry is string)
                        insert(dt, entry.ToString());
        }

        static void storeAllElement(AFElement element, DataTable dt)
        {
            addNameAndDescription(dt, element, element);
            foreach (AFAttribute attribute in element.Attributes)
                storeAllAttribute(attribute, dt);
            foreach (AFElement child in element.Elements)
                storeAllElement(child, dt);
        }

        static void storeAllAttribute(AFAttribute attr, DataTable dt)
        {
            addNameAndDescription(dt, attr, attr);
            foreach (AFAttribute attribute in attr.Attributes)
                storeAllAttribute(attribute, dt);
        }

        static AFTable createOrReturnTranslationTable(AFDatabase db)
        {
            AFTable translations = db.Tables[$"Translations_{language}"];
            if (translations == null)
            {
                translations = db.Tables.Add($"Translations_{language}");
                addLanguageTitles(translations);
                db.CheckIn();
            }
            return translations;
        }
        static void addLanguageTitles(AFTable table)
        {
            DataTable datatable = new DataTable();
            datatable.Columns.Add("en", typeof(System.String));
            datatable.Columns.Add(language, typeof(System.String));
            datatable.Columns.Add($"{language}_Check", typeof(System.Boolean));
            table.Table = datatable;
        }

        static void addNameAndDescription(DataTable dt, IAFNamedObject namedObject, IAFObjectDescription describedObject)
        {
            insert(dt, namedObject.Name);
            insert(dt, describedObject.Description);
        }
        static void insert(DataTable dt, string word)
        {
            if (!dt.AsEnumerable().Any(row => word == row.Field<String>("en")))
            {
                DataRow row = dt.NewRow();
                row["en"] = word;
                if (word != "")
                {
                    row[language] = translate(word, language);
                }
                dt.Rows.Add(row);
            }
            usedWords.Add(word);
        }
    }
}
