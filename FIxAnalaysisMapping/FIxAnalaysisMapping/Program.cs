using OSIsoft.AF;

namespace FIxAnalaysisMapping
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new PISystems().DefaultPISystem.Databases.DefaultDatabase;
            fixAnalysis(db);
        }
        public static void fixAnalysis(AFDatabase db)
        {
            foreach (var analysis in db.AnalysisTemplates)
            {
                if (analysis.AnalysisRulePlugIn.Name == "PerformanceEquation")
                    analysis.AnalysisRule.RefreshConfigurationAndVariableMapping();
                if (analysis.AnalysisRulePlugIn.Name == "EventFrame")
                    foreach (var rule in analysis.AnalysisRule.AnalysisRules)
                        rule.RefreshConfigurationAndVariableMapping();
                analysis.CheckIn();
            }
        }
    }
}
