using OSIsoft.AF;
using OSIsoft.AF.Analysis;
using OSIsoft.AF.Search;

namespace TurnOffAnalyses
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new PISystems().DefaultPISystem.Databases.DefaultDatabase;
            var analyses = new AFAnalysisSearch(db, null, "*");
            foreach (var analysis in analyses.FindAnalyses())
                analysis.SetStatus(AFStatus.Disabled);
        }
    }
}
