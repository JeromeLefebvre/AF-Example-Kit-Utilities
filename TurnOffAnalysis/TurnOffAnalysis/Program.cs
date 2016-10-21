using OSIsoft.AF.Analysis;
using OSIsoft.AF;

namespace TurnOffAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new PISystems().DefaultPISystem.Databases.DefaultDatabase;
            foreach (var analysis in db.Analyses)
                analysis.SetStatus(AFStatus.Disabled);
        }
    }
}
