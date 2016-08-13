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

namespace AllNamesAndDescriptions
{
    class Program
    {
        private static TranslateService service = new TranslateService(new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyDwFN_wQqfrf_m79ek8UAraSYA2rRbRohI", // your API key, that you get from Google Developer Console
                                                                //ApplicationName = "jlefebvrenew" // your application name, that you get form Google Developer Console
        });
        static void Main(string[] args)
        {
            PISystem system = new PISystems().DefaultPISystem;
            AFDatabase kitdb = system.Databases.DefaultDatabase;
            CreateEnglishTable(kitdb);

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

        static void CreateEnglishTable(AFDatabase db)
        {
            AFTable translation = createOrReturnTranslationTable(db);
            DataTable dt = translation.Table;
            
            // Get all elements and all attributes
            foreach(AFElement element in db.Elements)
            {
                storeAllElement(element, dt);
                db.CheckIn();
            }
            // Get all tables
            foreach(AFTable table in db.Tables)
            {
                if (table != translation)
                    storeAllTableHeaders(table, dt);
            }
            db.CheckIn();
            foreach (AFElementTemplate elemtemplate in db.ElementTemplates)
            {
                insert(dt, elemtemplate.Name);
                insert(dt, elemtemplate.NamingPattern);
            }
            db.CheckIn();
            foreach (AFCategory category in db.AnalysisCategories)
                storeCategory(category, dt);
            foreach (AFCategory category in db.ElementCategories)
                storeCategory(category, dt);
            foreach (AFCategory category in db.AttributeCategories)
                storeCategory(category, dt);
            foreach (AFCategory category in db.TableCategories)
                storeCategory(category, dt);
        }

        static void storeCategory(AFCategory category, DataTable dt)
        {
            insert(dt, category.Name);
            insert(dt, category.Description);
        }
        static void storeAllTableHeaders(AFTable table, DataTable dt)
        {
            foreach (DataColumn column in table.Table.Columns)
                insert(dt, column.ColumnName);
        }

        static void storeAllElement(AFElement element, DataTable dt)
        {
            AddElementInformation(dt, element);
            foreach (AFAttribute attribute in element.Attributes)
            {
                storeAllAttribute(attribute, dt);
            }
            foreach (AFElement child in element.Elements)
            {
                storeAllElement(child, dt);
            }
        }

        static void storeAllAttribute(AFAttribute attr, DataTable dt)
        {
            AddAttributeInformation(dt, attr);
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
            datatable.Columns.Add("Chinese", typeof(System.String));
            table.Table = datatable;
        }

        static void AddAttributeInformation(DataTable dt, AFAttribute attr)
        {
            insert(dt, attr.Name);
            insert(dt, attr.Description);
        }

        static void AddElementInformation(DataTable dt, AFElement elem)
        {
            insert(dt, elem.Name);
            insert(dt, elem.Description);
        }
        static void insert(DataTable dt, string word)
        {
            if (!dt.AsEnumerable().Any(row => word == row.Field<String>("English")))
            {
                DataRow row = dt.NewRow();
                row["English"] = word;
                if (word != "") { 
                    row["Japanese"] = translate(word, "ja");
                }
                dt.Rows.Add(row);
            }
        }
    }
}
