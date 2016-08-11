using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.PI;
using OSIsoft.AF.Asset;


namespace ResetToTemplate
{
    class Program
    {
        static void Main(string[] args)
        {
            PISystem local = PISystem.CreatePISystem("localhost");
            resetToTemplate(local.Databases["Kit"]);
            //AFDatabaseEditsCheck.runTests(local, resetToTemplate);
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
    }
}
