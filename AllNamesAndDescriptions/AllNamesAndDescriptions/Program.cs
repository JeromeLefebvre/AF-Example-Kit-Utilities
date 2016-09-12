using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF.Asset;
using OSIsoft.AF;
using System.Data;
using Google.Apis.Translate.v2;
using Google.Apis.Translate.v2.Data;
using Google.Apis.Services;
using static Google.Apis.Translate.v2.TranslationsResource;
using System.Text;
using System.IO;

namespace AllNamesAndDescriptions
{
    class Program
    {
        public static string keyPath = @"c:\apikey.txt";
        private static TranslateService service = new TranslateService(new BaseClientService.Initializer()
        {
            //ApiKey = File.ReadAllText(keyPath), // your API key, that you get from Google Developer Console
            ApiKey = "",
            ApplicationName = "jlefebvrenew" // your application name, that you get form Google Developer Console
        });
        static void Main(string[] args)
        {
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

        static void fillEnglishTable(AFDatabase db)
        {
            AFTable translation = createOrReturnTranslationTable(db);
            DataTable dt = translation.Table;

            insert(dt, db.Description);
            foreach (AFElement element in db.Elements)
                storeAllElement(element, dt);
            db.CheckIn();

            foreach (AFTable table in db.Tables)
                if (table != translation && table.Name != "UOM Conversion")
                    storeAllTableHeaders(table, dt);

            db.CheckIn();
            foreach (AFElementTemplate elem in db.ElementTemplates)
            {
                addNameAndDescription(dt, elem, elem);
                insert(dt, elem.NamingPattern);
            }
            foreach (var analysis in db.AnalysisTemplates)
                addNameAndDescription(dt, analysis, analysis);
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
            AFTable translations = db.Tables["Translations"];
            if (translations == null)
            {
                translations = db.Tables.Add("Translations");
                addLanguageTitles(translations);
                db.CheckIn();
            }
            return translations;
        }
        static void addLanguageTitles(AFTable table)
        {
            DataTable datatable = new DataTable();
            datatable.Columns.Add("English", typeof(System.String));
            datatable.Columns.Add("Japanese", typeof(System.String));
            datatable.Columns.Add("Check", typeof(System.Boolean));
            table.Table = datatable;
        }

        static void addNameAndDescription(DataTable dt, IAFNamedObject namedObject, IAFObjectDescription describedObject)
        {
            insert(dt, namedObject.Name);
            insert(dt, describedObject.Description);
        }
        static void insert(DataTable dt, string word)
        {
            if (!dt.AsEnumerable().Any(row => word == row.Field<String>("English")))
            {
                DataRow row = dt.NewRow();
                row["English"] = word;
                if (word != "")
                {
                    row["Japanese"] = translate(word, "ja");
                }
                dt.Rows.Add(row);
            }
        }
    }
}
