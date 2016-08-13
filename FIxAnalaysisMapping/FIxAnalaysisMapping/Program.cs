using System;
using OSIsoft.AF;
using OSIsoft.AF.Analysis;

namespace FIxAnalaysisMapping
{
    class Program
    {
        static void Main(string[] args)
        {
            PISystem local = new PISystems().DefaultPISystem;
            AFDatabase db = local.Databases.DefaultDatabase;
            fixAnalysis(db);
        }
        public static bool fixAnalysis(AFDatabase db)
        {
            foreach (AFAnalysisTemplate analysis in db.AnalysisTemplates)
            {
                analysis.AnalysisRule.RefreshConfigurationAndVariableMapping();
                analysis.CheckIn();
            }
            return true;
        }
    }
}
