﻿using OSIsoft.AF;

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
                analysis.AnalysisRule.RefreshConfigurationAndVariableMapping();
                analysis.CheckIn();
            }
        }
    }
}
