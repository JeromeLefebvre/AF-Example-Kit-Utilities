using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF.Asset;
using OSIsoft.AF;
using System.Data;
using OSIsoft.AF.Analysis;

namespace AllNamesAndDescriptions
{
    class Program
    {

        static void Main(string[] args)
        {
            PISystem system = PISystem.CreatePISystem("localhost");
            AFDatabase kitdb = system.Databases["Database85"];
            SwitchToJapanese(kitdb);

            // Create the translation service.

            kitdb.CheckIn();
            Console.ReadLine();
        }

        static void SwitchToJapanese(AFDatabase db)
        {
            AFTable translations = createOrReturnTranslationLibrary(db);

            // Get all elements and all attributes
            foreach (AFElement element in db.Elements)
            {
                translateElementAndChild(element, translations.Table);
                db.CheckIn();
            }
            // Get all tables
            foreach (AFTable table in db.Tables)
            {
                if (table != translations)
                    translateTableHeaders(table, translations.Table);
            }
            db.CheckIn();
            foreach (AFElementTemplate elementTemplate in db.ElementTemplates)
            {
                translateElementTemplate(elementTemplate, translations.Table, db);
                //insert(translations.Table, elemtemplate.NamingPattern);
                db.CheckIn();
            }

        }

        static void translateTableHeaders(AFTable table, DataTable translations)
        {
            foreach (DataColumn column in table.Table.Columns)
            {
                column.ColumnName = translate(translations, column.ColumnName, "Japanese");
            }
        }

        static void translateElementAndChild(AFElement element, DataTable translations)
        {

            translateElement(element, translations);

            foreach (AFElement child in element.Elements)
            {
                translateElementAndChild(child, translations);
            }
        }

        static void translateElementTemplate(AFElementTemplate elem, DataTable dt, AFDatabase db)
        {
            elem.Name = translate(dt, elem.Name, "Japanese");
            elem.Description = translate(dt, elem.Description, "Japanese");
            foreach (AFAttributeTemplate attribute in elem.AttributeTemplates)
            {
                translateAttribute(elem, attribute, dt);
                db.CheckIn();
            }
        }
        static void translateElement(AFElement elem, DataTable dt)
        {
            elem.Name = translate(dt, elem.Name, "Japanese");
            elem.Description = translate(dt, elem.Description, "Japanese");
        }

        static void translateAttribute(AFElementTemplate elem, AFAttributeTemplate attr, DataTable dt)
        {
            AFVariableMappingData variableMapping = null;
            AFAnalysisTemplate analysisTargetingAttribute = null;
            foreach (AFAnalysisTemplate analysis in elem.AnalysisTemplates)
            {
                analysis.AnalysisRule.VariableMap.TryGetMapping(attr.Name, out variableMapping);
                if (variableMapping != null)
                {
                    analysisTargetingAttribute = analysis;
                    break;
                }
            }

            attr.Name = translate(dt, attr.Name, "Japanese");
            attr.Description = translate(dt, attr.Description, "Japanese");
            elem.CheckIn();
            if (analysisTargetingAttribute != null)
            {
                //analysisTargetingAttribute.AnalysisRule.VariableMap.SetMapping(attr.Name, variableMapping);
                analysisTargetingAttribute.AnalysisRule.VariableMap.RefreshMappings(elem);
            }
        }


        static AFTable createOrReturnTranslationLibrary(AFDatabase db)
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

        static void insert(DataTable dt, string word)
        {
            if (!dt.AsEnumerable().Any(row => word == row.Field<String>("English")))
            {
                DataRow row = dt.NewRow();
                row["English"] = word;
                if (word != "")
                {
                    row["Japanese"] = translate(dt, word, "ja");
                }
                dt.Rows.Add(row);
            }
        }

        static string escapeForSQL(string word)
        {
            return word.Replace("'", "''");

        } 
        static string translate(DataTable dt, string word, string language)
        {
            DataRow[] result = dt.Select($"English = '{escapeForSQL(word)}'");
            if (word == "") return "";
            if (result.Count() > 0)
            {
                return (string)result[0]["Japanese"];
            }
            return word;
        }
    }
}
