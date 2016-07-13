using OSIsoft.AF;
using OSIsoft.AF.Asset;

namespace ConvertTagNamingConvention
{
    class Program
    {
        static void Main(string[] args)
        {
            PISystem local = PISystem.CreatePISystem("localhost");
            AFDatabase db = local.Databases["FreshKit13"];
            foreach (AFElementTemplate template in db.ElementTemplates)
            {
                foreach (AFAttributeTemplate attr in template.AttributeTemplates)
                {
                    AFAttributeTemplate child = attr.AttributeTemplates["Tagname"];

                    if (child == null)
                        continue;
                    child.Name = "タグ名";
                    child.Description = "属性のタグ名";
                    child.ConfigString = @"OSIDEMO_;'.|エレメントの英語名';""."";'.|属性の英語名';""."";""%AttributeID%"";";
                    child.IsHidden = true;
                    AFAttributeTemplate elementLookup = child.AttributeTemplates.Add("エレメントの英語名");
                    elementLookup.Description = "エレメントの名前を英語に翻訳する";
                    elementLookup.DataReferencePlugIn = local.DataReferencePlugIns["Table Lookup"];
                    elementLookup.DataReference.ConfigString = @"SELECT English FROM Translations WHERE Japanese = '%Element%';RWM=%Element%";
                    elementLookup.IsHidden = true;

                    AFAttributeTemplate attributelookup = child.AttributeTemplates.Add("属性の英語名");
                    attributelookup.DataReferencePlugIn = local.DataReferencePlugIns["Table Lookup"];
                    attributelookup.Description = "属性の名前を英語に翻訳する";
                    attributelookup.DataReference.ConfigString = @"SELECT English FROM Translations WHERE Japanese = '%..|attribute%';RWM=%..|..|Attribute%";
                    attributelookup.IsHidden = true;
                }
            }
            // Not easy to directly access a PI Point Data refence's TagName property from the AFAttributeTemplate
            // Thus, working around it by doing a string replace
            string xml = local.ExportXml(db, PIExportMode.AllReferences);
            xml = xml.Replace("|TagName", "|タグ名");
            local.ImportXml(db, PIImportMode.AllowUpdate, xml);
            db.CheckIn();
        }
    }
}
