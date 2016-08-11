using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.PI;
using OSIsoft.AF.Asset;

using OSIsoft.AF.Analysis;
using System.Xml;
using System.IO;

namespace ResetToTemplate
{
    class Program
    {
        static void Main(string[] args)
        {
            PISystem local = PISystem.CreatePISystem("localhost");
            runTests(local, resetToTemplate);
            Console.ReadLine();
        }

        public static bool resetToTemplate(AFDatabase db)
        {
            // Special case in all attributes of the PI Data Archive elemnt is reset to template
            AFElement PIDataArchive = db.Elements["PI Data Archive"];
            if (PIDataArchive != null)
                foreach (AFAttribute attr in PIDataArchive.Attributes)
                    attr.ResetToTemplate();

            foreach(AFElement elem in db.Elements)
            {
                resetElement(elem);
            }
            return true;
        }
        public static void resetElement(AFElement elem)
        {
            foreach(AFAttribute attr in elem.Attributes)
            {
                if (!attr.Template.IsConfigurationItem)
                    resetAttribute(attr);
            }
            elem.CheckIn();
            foreach (AFElement child in elem.Elements)
                resetElement(child);
        }
        public static void resetAttribute(AFAttribute attr)
        {
            attr.ResetToTemplate();
            foreach(AFAttribute childAttr in attr.Attributes)
            {
                resetAttribute(childAttr);
            }
        }

        public static bool compareDB(AFDatabase first, AFDatabase second)
        {
            string firstXML = getXML(first);
            string secondXML = getXML(second);

            if (firstXML == secondXML)
                return true;
            else
            {
                // Output the first miss match:
                int difference = GetFirstBreakIndex(firstXML, secondXML, true);
                Console.WriteLine(firstXML.Substring(difference - 150, 250));
                Console.WriteLine(secondXML.Substring(difference - 150, 250));
                return false;
            }
        }

        public static string getXML(AFDatabase db)
        {
            string dbXML = db.PISystem.ExportXml(db.Elements, PIExportMode.AllReferences | PIExportMode.NoUniqueID);

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(dbXML);

            XmlNodeList xnList = xml.SelectNodes("/AF");
            return xnList[0].InnerXml;
        }
        public static void runTests(PISystem system, Func<AFDatabase, bool> updateDB)
        {
            string testDirectory = @"C:\Users\jlefebvre\Documents\GitHub\AF-Example-Kit-Utilities\ResetToTemplate\ResetToTemplate\testDB\";
            string[] fileEntries = Directory.GetFiles(testDirectory + @"source\");
            foreach (string sourceFile in fileEntries)
            {
                string sourceFileName = Path.GetFileName(sourceFile);
                string targetFile = testDirectory + @"target\" + sourceFileName;
                AFDatabase source = createDB(system, File.ReadAllText(sourceFile), "source");
                AFDatabase target = createDB(system, File.ReadAllText(targetFile), "target");
                updateDB(source);
                bool check = compareDB(source, target);
                Console.WriteLine($"{sourceFileName} : {check}");
                if (check)
                {
                    system.Databases.Remove(source);
                    system.Databases.Remove(target);
                    system.CheckIn();
                }
                else
                {
                    Console.WriteLine($"Error dealing with: {sourceFileName}");
                    Console.ReadLine();
                    return;
                }
            }
        }

        public static AFDatabase createDB(PISystem system, string xml, string name)
        {
            AFDatabase db = system.Databases.Add(name);
            system.ImportXml(db, PIImportMode.AllowCreate, xml);
            db.CheckIn();
            return db;
        }

        public static int GetFirstBreakIndex(string a, string b, bool handleLengthDifference)
        {
            int equalsReturnCode = -1;
            if (String.IsNullOrEmpty(a) || String.IsNullOrEmpty(b))
            {
                return handleLengthDifference ? 0 : equalsReturnCode;
            }

            string longest = b.Length > a.Length ? b : a;
            string shorten = b.Length > a.Length ? a : b;
            for (int i = 0; i < shorten.Length; i++)
            {
                if (shorten[i] != longest[i])
                {
                    return i;
                }
            }

            // Handles cases when length is different (a="1234", b="123")
            // index=3 would be returned for this case
            if (handleLengthDifference && a.Length != b.Length)
            {
                return shorten.Length;
            }

            return equalsReturnCode;
        }
    }
}
