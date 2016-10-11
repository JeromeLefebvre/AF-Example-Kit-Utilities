using OSIsoft.AF;
using OSIsoft.AF.Asset;
using System.Data;
using System.Linq;

namespace ConvertTagNamingConvention
{
    class Program
    {
        const string tagname = "TagName";
        static void Main(string[] args)
        {
            PISystem system = new PISystems().DefaultPISystem;
            AFDatabase db = system.Databases.DefaultDatabase;
            convertTagNaming(db);
        }

        public static bool convertTagNaming(AFDatabase db)
        {
            AFCategory tagnameCategory = db.AttributeCategories[tagname];
            if (tagnameCategory == null)
                tagnameCategory = db.AttributeCategories.Add(tagname);

            foreach (AFElementTemplate template in db.ElementTemplates)
            {
                int depth = lookUpDepth(template);

                foreach (AFAttributeTemplate attr in template.AttributeTemplates)
                {
                    AFAttributeTemplate child = attr.AttributeTemplates[tagname];
                    
                    if (child == null)
                        continue;
                    attr.ConfigString = attr.ConfigString.Replace(tagname, "タグ名");

                    child.Name = "タグ名";
                    child.Description = "属性のタグ名";
                    string configString = @"'.|属性の英語名';";
                    child.IsHidden = true;

                    AFAttributeTemplate attributelookup = child.AttributeTemplates.Add("属性の英語名");
                    attributelookup.Categories.Add(tagnameCategory);
                    attributelookup.DataReferencePlugIn = db.PISystem.DataReferencePlugIns["Table Lookup"];
                    attributelookup.Description = "属性の名前を英語に翻訳する";
                    attributelookup.DataReference.ConfigString = @"SELECT English FROM Translations WHERE Japanese = '%..|..|attribute%';RWM=%..|..|Attribute%";
                    attributelookup.IsHidden = true;

                    string attributeName = "エレメントの英語名";
                    string description = "エレメントの名前を英語に翻訳する";
                    string elementField = "Element%";

                    for (int i = 0; i <= depth; i++)
                    {
                        configString = $@"'.|{attributeName}';""."";" + configString;
                        AFAttributeTemplate elementLookup = child.AttributeTemplates.Add(attributeName);
                        elementLookup.Categories.Add(tagnameCategory);
                        elementLookup.Description = description;
                        elementLookup.DataReferencePlugIn = db.PISystem.DataReferencePlugIns["Table Lookup"];
                        elementLookup.DataReference.ConfigString = $"SELECT English FROM Translations WHERE Japanese = '%{elementField}';RWM=%{elementField}";
                        elementLookup.IsHidden = true;
                        attributeName = "親の" + attributeName;
                        description = "親の" + description;
                        elementField = @"..\" + elementField;
                    }
                    child.ConfigString = @"OSIDEMO_;" + configString + @";""."";%..|AttributeID%";
                }
            }

            db.CheckIn();
            return true;
        }

        public static int lookUpDepth(AFElementTemplate template)
        {
            try
            {
                var elements = template.FindInstantiatedElements(true, AFSortField.Name, AFSortOrder.Ascending, 1);
                var element = (AFElement)elements[0];
                var parents = 0;
                while (element.Parent != null)
                {
                    parents++;
                    element = element.Parent;
                }
                return parents;
            }
            catch
            {
                return 0;
            }
        }
    }
}
