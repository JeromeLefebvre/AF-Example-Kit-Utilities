using OSIsoft.AF;
using OSIsoft.AF.Analysis;

namespace TurnOffAnalyses
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new PISystems().DefaultPISystem.Databases.DefaultDatabase;
            var analyses = AFAnalysis.FindAnalyses(db, "*", AFSearchField.Name, AFSortField.Name, AFSortOrder.Ascending, int.MaxValue);
            foreach (var analysis in analyses)
                analysis.SetStatus(AFStatus.Disabled);
        }
    }
}
