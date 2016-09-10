using System;
using OSIsoft.AF;
using OSIsoft.AF.Analysis;
using OSIsoft.AF.Asset;

namespace ResetToTemplate
{
    class Program
    {
        static void Main(string[] args)
        {
            PISystem local = new PISystems().DefaultPISystem;
            resetToTemplate(local.Databases.DefaultDatabase);
            //AFDatabaseEditsCheck.runTests(local, resetToTemplate);
        }

        public static bool resetToTemplate(AFDatabase db)
        {
            // Special case in all attributes of the PI Data Archive elemnt is reset to template
            AFElement PIDataArchive = db.Elements["PI Data Archive"];
            if (PIDataArchive != null)
                foreach (AFAttribute attr in PIDataArchive.Attributes)
                    attr.ResetToTemplate();

            foreach(AFElement elem in db.Elements)
                resetElement(elem);
            return true;
        }
        public static void resetElement(AFElement elem)
        {
            foreach(AFAttribute attr in elem.Attributes)
               resetAttribute(attr);
            foreach (AFElement child in elem.Elements)
                resetElement(child);
            foreach (AFAnalysis analysis in elem.Analyses)
                resetAnalysis(analysis);
            elem.CheckIn();
        }
        public static void resetAttribute(AFAttribute attr)
        {
            if (!attr.IsConfigurationItem && !attr.Template.IsConfigurationItem)
                attr.ResetToTemplate();
            //if (attr.DataReference != null && attr.DataReferencePlugIn.Name == "PI Point")
            //    attr.DataReference.CreateConfig();
            foreach(AFAttribute childAttr in attr.Attributes)
                resetAttribute(childAttr);
        }
        public static void resetAnalysis(AFAnalysis analysis)
        {
            analysis.Description = analysis.Template.Description;
            analysis.CheckIn();
        }
    }
}
