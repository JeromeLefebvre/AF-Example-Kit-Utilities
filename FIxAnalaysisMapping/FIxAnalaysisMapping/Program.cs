using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.Analysis;
using OSIsoft.AF.Asset;

namespace FIxAnalaysisMapping
{
    class Program
    {
        static void Main(string[] args)
        {
            PISystem local = PISystem.CreatePISystem("localhost");
            AFDatabase db = local.Databases["Database90"];
            fixAnalysis(db);
        }
        public static bool fixAnalysis(AFDatabase db)
        {
            foreach (AFAnalysisTemplate analysis in db.AnalysisTemplates)
            {
                AFVariableMap map = analysis.AnalysisRule.VariableMap;
                Console.WriteLine(map.ToString());
                analysis.AnalysisRule.RefreshConfigurationAndVariableMapping();
                map = analysis.AnalysisRule.VariableMap;
                Console.WriteLine(map.ToString());
                // Can't only get away with checkin at the db level.
                analysis.CheckIn();
            }
            return true;
        }
    }
}
