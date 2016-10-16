using OSIsoft.AF;
using OSIsoft.AF.Asset;

using CommandLine;
using CommandLine.Text;

namespace ConvertTagNamingConvention
{
    class Program
    {
        const string tagname = "TagName";
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                // Command values are available here
                language = options.language;
            }
                PISystem system = new PISystems().DefaultPISystem;
            AFDatabase db = system.Databases.DefaultDatabase;
            convertTagNaming(db);
        }

        public static string language;
        class Options
        {
            [Option('l', "language", Required = true, DefaultValue = "ja",
              HelpText = "The language used in the translation")]
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
                    //attr.ConfigString = attr.ConfigString.Replace(tagname, "タグ名");

                    child.Name = tagname;
                    child.Description = "The name of the attribute";
                    string configString = @"'.|The English Name of the Attribute';";
                    child.IsHidden = true;

                    AFAttributeTemplate attributelookup = child.AttributeTemplates.Add("The English Name of the Attribute");
                    attributelookup.Categories.Add(tagnameCategory);
                    attributelookup.DataReferencePlugIn = db.PISystem.DataReferencePlugIns["Table Lookup"];
                    attributelookup.Description = "The English translation of the attribute's name";
                    var dummy = $" Translations_{language}";
                    attributelookup.DataReference.ConfigString = $"SELECT en FROM [Translations_{language}] WHERE {language} = '%..|..|attribute%';RWM=%..|..|Attribute%";
                    attributelookup.IsHidden = true;

                    string attributeName = "The English name of the Element";
                    string description = "The English translation of the element's name";
                    string elementField = "Element%";

                    for (int i = 0; i <= depth; i++)
                    {
                        configString = $@"'.|{attributeName}';""."";" + configString;
                        AFAttributeTemplate elementLookup = child.AttributeTemplates.Add(attributeName);
                        elementLookup.Categories.Add(tagnameCategory);
                        elementLookup.Description = description;
                        elementLookup.DataReferencePlugIn = db.PISystem.DataReferencePlugIns["Table Lookup"];
                        elementLookup.DataReference.ConfigString = $"SELECT en FROM [Translations_{language}] WHERE {language} = '%{elementField}';RWM=%{elementField}";
                        elementLookup.IsHidden = true;
                        if (attributeName.Contains("Parent"))
                            attributeName = attributeName.Replace("Parent", "Great-Parent");
                        else
                            attributeName = attributeName.Replace("Element", "Parent Element");
                        
                        // description = "親の" + description;
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
