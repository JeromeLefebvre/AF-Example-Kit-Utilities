using OSIsoft.AF;
using OSIsoft.AF.Analysis;
using OSIsoft.AF.Search;
using OSIsoft.AF.Time;

using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;
using System;

namespace TurnOffAnalyses
{
    class Program
    {

        public static AFTime startTime = new AFTime("Sun");
        public static AFTime endTime = new AFTime("*");

        public static AFTimeRange timeRange = new AFTimeRange(startTime, endTime);

        public static int sleepTimePerAnalysis = 1*100; // in milliseconds
        static void Main(string[] args)
        {
            var af = new PISystems().DefaultPISystem;
            var db = af.Databases.DefaultDatabase;
            var service = af.AnalysisService;

            StreamReader file = new StreamReader("c:\\kit\\utilities.txt");
            string line;

            while ((line = file.ReadLine()) != null)
            {
                Console.WriteLine(line);
                var search = new AFAnalysisSearch(db, null, line);

                
                IEnumerable<AFAnalysis> analyses = search.FindAnalyses();
                Console.WriteLine($"Found: {analyses.Count()} analyses.");
                
                
                // start all found analyses
                foreach (var analysis in analyses)
                    analysis.SetStatus(AFStatus.Enabled);
                
                // status does not return information in this version
                var status = service.QueueCalculation(analyses, timeRange, AFAnalysisService.CalculationMode.DeleteExistingData);
                Thread.Sleep(sleepTimePerAnalysis * analyses.Count());
            }
            file.Close();
        }
    }
}
